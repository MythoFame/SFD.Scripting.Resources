using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Helpers for loading and rendering Base64 frame data images.
    /// Format per frame: Base64 of [width:u32][height:u32][pixels: w*h*3 RGB],
    /// little-endian, row-major, top-left origin. No alpha; text objects
    /// don't support transparency. Frames are joined by '_'.
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// A single decoded image frame: raw RGB pixels with its dimensions.
        /// </summary>
        public class DecodedImage
        {
            public Color[] Pixels = [];
            public int Width;
            public int Height;
        }

        /// <summary>Decoded frames by FNV-1a hash of the frame string.</summary>
        private static readonly Dictionary<int, DecodedImage> _cache = [];

        /// <summary>Inverse batch scaling: fewer pixels render in bigger batches.</summary>
        private const int BATCH_BUDGET = 65536;

        private const int MIN_BATCH = 64;
        private const int MAX_BATCH = 2048;

        private class RenderJob
        {
            public DecodedImage[] Frames;
            public IObjectText[] Pixels;
            public int CurrentFrame;
            public Vector2 Position;
            public string ObjectId;
            public int Uid;
            public int Next;
            public int Batch;
            public Events.UpdateCallback Callback;
            public Events.UpdateCallback AnimCallback;
        }

        /// <summary>
        /// Returns all frames of image data, decoding and caching on first
        /// use. Returns null when the data is missing or has no valid frames.
        /// </summary>
        public static DecodedImage[] GetFrames(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;

            List<DecodedImage> frames = [];

            foreach (string part in data.Split('_'))
            {
                if (string.IsNullOrEmpty(part)) continue;

                int key = HashString(part);

                if (_cache.TryGetValue(key, out DecodedImage cached))
                {
                    frames.Add(cached);
                    continue;
                }

                DecodedImage image = DecodeFrame(part);

                if (image == null) return null;

                _cache[key] = image;
                frames.Add(image);
            }

            return frames.Count > 0 ? [.. frames] : null;
        }

        /// <summary>
        /// Starts rendering an image as text objects centered at a position,
        /// top row first, creating a batch of pixels per update tick. When
        /// multiple frames exist, colors animate on a fixed timer afterwards.
        /// Returns the total pixel count.
        /// </summary>
        public static int Render(DecodedImage[] frames, Vector2 position, string objectId, int uid)
        {
            DecodedImage first = frames[0];
            int total = first.Width * first.Height;

            var job = new RenderJob
            {
                Frames = frames,
                Pixels = new IObjectText[total],
                Position = position,
                ObjectId = objectId,
                Uid = uid,
                Batch = GetBatchSize(total)
            };

            job.Callback = Game.Events.StartUpdateCallback(_ => PumpBatch(job));

            return total;
        }

        private static int GetBatchSize(int total) =>
            Math.Clamp(BATCH_BUDGET / total, MIN_BATCH, MAX_BATCH);

        private static void PumpBatch(RenderJob job)
        {
            DecodedImage first = job.Frames[0];
            int total = job.Pixels.Length;
            int end = Math.Min(job.Next + job.Batch, total);
            float halfWidth = (first.Width - 1) / 2f;
            float halfHeight = (first.Height - 1) / 2f;

            for (; job.Next < end; job.Next++)
            {
                int x = job.Next % first.Width;
                int y = job.Next / first.Width;

                if (Game.CreateObject(job.ObjectId, job.Position + new Vector2(x - halfWidth, halfHeight - y)) is not IObjectText pixel) continue;

                pixel.SetText(".");
                pixel.SetTextColor(first.Pixels[job.Next]);
                job.Pixels[job.Next] = pixel;
            }

            if (job.Next < total) return;

            job.Callback.Stop();
            job.Callback = null;

            Game.ShowChatMessage("Image rendered.", Color.Green, job.Uid);

            if (job.Frames.Length > 1)
                job.AnimCallback = Game.Events.StartUpdateCallback(_ => Animate(job), AnimInterval);
        }

        private const uint AnimInterval = 100;

        private static void Animate(RenderJob job)
        {
            job.CurrentFrame = (job.CurrentFrame + 1) % job.Frames.Length;

            DecodedImage frame = job.Frames[job.CurrentFrame];

            if (frame.Pixels.Length != job.Pixels.Length)
            {
                job.AnimCallback.Stop();
                job.AnimCallback = null;
                return;
            }

            for (int i = 0; i < job.Pixels.Length; i++)
            {
                var pixel = job.Pixels[i];

                if (pixel == null || pixel.IsRemoved)
                {
                    job.AnimCallback.Stop();
                    job.AnimCallback = null;
                    return;
                }

                pixel.SetTextColor(frame.Pixels[i]);
            }
        }

        private static DecodedImage DecodeFrame(string frame)
        {
            byte[] data;

            try
            {
                data = Convert.FromBase64String(frame);
            }
            catch (FormatException)
            {
                return null;
            }

            if (data.Length < 8) return null;

            int width = BitConverter.ToInt32(data, 0);
            int height = BitConverter.ToInt32(data, 4);

            if (width < 1 || height < 1) return null;
            if ((long)width * height * 3 + 8 != data.Length) return null;

            Color[] pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                int o = 8 + i * 3;
                pixels[i] = new Color(data[o], data[o + 1], data[o + 2]);
            }

            return new DecodedImage { Pixels = pixels, Width = width, Height = height };
        }

        private static int HashString(string value)
        {
            uint hash = 2166136261;

            foreach (char c in value)
            {
                hash ^= c;
                hash *= 16777619;
            }

            return (int)hash;
        }
    }
}
