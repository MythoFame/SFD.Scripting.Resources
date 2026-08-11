using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Helpers for managing bot companions in the main team (<see cref="MAIN_TEAM"/>),
    /// used in official-campaign-style map scripts where missing players are replaced
    /// by bot companions. Provides companion lookup, damage tuning, automatic follow
    /// behavior, and trigger-callable helpers (<see cref="CompanionsFollow"/>, etc.).
    /// </summary>
    public static class Companions
    {
        private const uint CHECK_COOLDOWN = 1000;
        private const float BOT_COMPANION_DMG_MULT = 0.5f;

        /// <summary>
        /// The team that companions (and the players they follow) belong to.
        /// </summary>
        public const PlayerTeam MAIN_TEAM = PlayerTeam.Team1;

        /// <summary>
        /// Custom ID of the object companions are sent to by <see cref="GoToSaferoom"/>.
        /// </summary>
        public const string GUARD_TARGET_ID = "SaferoomGuardTarget";

        private static bool _initialized = false;

        private static float _previousDiff = -1;

        /// <summary>
        /// Initializes companion management: halves melee and projectile damage dealt by
        /// all current companions and starts the periodic update callback. Safe to call
        /// more than once; only the first invocation has any effect.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;

            _initialized = true;

            foreach (IPlayer companion in GetCompanions)
            {
                PlayerModifiers modifiers = companion.GetModifiers();

                modifiers.MeleeDamageDealtModifier *= BOT_COMPANION_DMG_MULT;
                modifiers.ProjectileDamageDealtModifier *= BOT_COMPANION_DMG_MULT;

                companion.SetModifiers(modifiers);
            }

            Game.Events.StartUpdateCallback(OnUpdate, CHECK_COOLDOWN);
        }

        /// <summary>
        /// All living, non-null bot players on the <see cref="MAIN_TEAM"/>.
        /// </summary>
        public static IPlayer[] GetCompanions =>
            [.. from Q in Game.GetPlayers() where Q != null && Q.GetTeam() == MAIN_TEAM && !Q.IsDead && Q.IsBot select Q];

        /// <summary>
        /// Attempts to set the bot name of the <paramref name="index"/>-th user on the
        /// <see cref="MAIN_TEAM"/>. Returns <c>false</c> when there is no user at that index.
        /// </summary>
        /// <param name="index">Index of the user (by game order) to rename.</param>
        /// <param name="name">The bot name to assign.</param>
        public static bool SetBotName(int index, string name)
        {
            List<IPlayer> localPlayers = [.. from Q in Game.GetPlayers() where Q != null && Q.IsUser && Q.GetTeam() == MAIN_TEAM select Q];

            if (index < localPlayers.Count)
            {
                return localPlayers[index].SetBotName(name);
            }

            return false;
        }

        /// <summary>
        /// Periodic update started by <see cref="Initialize"/>. Applies the current
        /// game difficulty as the damage-taken modifier for all living main-team players
        /// whenever the difficulty changes, and keeps every companion guarding a living
        /// user player on the main team.
        /// </summary>
        private static void OnUpdate(float _)
        {
            float difficulty = Game.CurrentDifficulty;

            if (difficulty != _previousDiff)
            {
                foreach (IPlayer player in Game.GetPlayers())
                {
                    if (player.GetTeam() != MAIN_TEAM || player.IsDead) continue;

                    PlayerModifiers m = player.GetModifiers();

                    m.ExplosionDamageTakenModifier = Game.CurrentDifficulty;
                    m.ProjectileDamageTakenModifier = Game.CurrentDifficulty;
                    m.FireDamageTakenModifier = Game.CurrentDifficulty;
                    m.MeleeDamageTakenModifier = Game.CurrentDifficulty;

                    player.SetModifiers(m);
                }

                _previousDiff = difficulty;
            }

            IPlayer[] companions = GetCompanions;

            IPlayer playerToFollow =
                (from Q in Game.GetActiveUsers() where Q != null && !Q.IsBot && Q.GetPlayer() != null && !Q.GetPlayer().IsDead select Q.GetPlayer())
                .FirstOrDefault();

            foreach (IPlayer companion in companions)
            {
                IObject guardTarget = companion.GetGuardTarget();

                if (guardTarget == playerToFollow || guardTarget.CustomID == GUARD_TARGET_ID)
                {
                    continue;
                }

                companion.SetGuardTarget(playerToFollow);

                if (Game.IsEditorTest)
                {
                    Game.CreateDialogue($"Gonna follow {(playerToFollow == null ? "noone" : playerToFollow.Name)} now!", companion, duration: 500);
                }
            }
        }
    }

    /// <summary>
    /// Map-trigger helper that makes all <see cref="Companions.GetCompanions"/> follow a
    /// nearby target by raising their bot <c>ChaseRange</c> and <c>GuardRange</c>.
    /// </summary>
    /// <param name="args">The <see cref="TriggerArgs"/> supplied by the map trigger.</param>
    public static void CompanionsFollow(TriggerArgs args)
    {
        const float CHASE_RANGE = 82;
        const float GUARD_RANGE = 64;

        foreach (IPlayer companion in Companions.GetCompanions)
        {
            BotBehaviorSet set = companion.GetBotBehaviorSet();

            if (set == null)
            {
                continue;
            }

            set.ChaseRange = CHASE_RANGE;
            set.GuardRange = GUARD_RANGE;

            companion.SetBotBehaviorSet(set);

            if (Game.IsEditorTest)
            {
                Game.CreateDialogue("Following!", companion, duration: 500);
            }
        }
    }

    /// <summary>
    /// Map-trigger helper that switches all <see cref="Companions.GetCompanions"/> to
    /// aggressive behavior by zeroing their bot <c>ChaseRange</c> and <c>GuardRange</c>.
    /// </summary>
    /// <param name="args">The <see cref="TriggerArgs"/> supplied by the map trigger.</param>
    public static void CompanionsFight(TriggerArgs args)
    {
        foreach (IPlayer companion in Companions.GetCompanions)
        {
            BotBehaviorSet botBehaviorSet = companion.GetBotBehaviorSet();

            if (botBehaviorSet == null)
            {
                continue;
            }

            botBehaviorSet.ChaseRange = 0;
            botBehaviorSet.GuardRange = 0;

            companion.SetBotBehaviorSet(botBehaviorSet);

            if (Game.IsEditorTest)
            {
                Game.CreateDialogue("Fighting!", companion, duration: 500);
            }
        }
    }

    /// <summary>
    /// Map-trigger helper that sends all <see cref="Companions.GetCompanions"/> to the
    /// object with custom ID <see cref="Companions.GUARD_TARGET_ID"/>, with a moderate
    /// chase and guard range. Does nothing when the target object does not exist.
    /// </summary>
    /// <param name="args">The <see cref="TriggerArgs"/> supplied by the map trigger.</param>
    public static void GoToSaferoom(TriggerArgs args)
    {
        const float CHASE_RANGE = 64;
        const float GUARD_RANGE = 48;

        IObject target = Game.GetSingleObjectByCustomID(Companions.GUARD_TARGET_ID);

        if (target == null)
        {
            return;
        }

        foreach (IPlayer companion in Companions.GetCompanions)
        {
            companion.SetGuardTarget(target);

            BotBehaviorSet bbs = companion.GetBotBehaviorSet();

            bbs.ChaseRange = CHASE_RANGE;
            bbs.GuardRange = GUARD_RANGE;

            companion.SetBotBehaviorSet(bbs);

            if (Game.IsEditorTest)
            {
                Game.CreateDialogue("Going in saferoom!", companion, duration: 500f);
            }
        }
    }
}
