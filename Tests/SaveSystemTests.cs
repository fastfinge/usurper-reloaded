using Xunit;
using FluentAssertions;
using System.Text.Json;
using UsurperRemake.Systems;

namespace UsurperReborn.Tests;

/// <summary>
/// Save/Load round-trip tests - these actually find bugs
/// Tests that serialization preserves all data correctly
/// </summary>
public class SaveSystemTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
    };

    #region PlayerData Round-Trip Tests

    [Fact]
    public void PlayerData_RoundTrip_PreservesAllCoreStats()
    {
        // Arrange - Create a player with non-default values
        var original = new PlayerData
        {
            Id = "test-player-123",
            Name1 = "TestPlayer",
            Name2 = "The Brave",
            RealName = "John",
            Level = 42,
            Experience = 123456789L,
            HP = 500,
            MaxHP = 750,
            Gold = 99999L,
            BankGold = 500000L,
            Strength = 150,
            Defence = 100,
            Stamina = 80,
            Agility = 90,
            Charisma = 60,
            Dexterity = 85,
            Wisdom = 70,
            Intelligence = 95,
            Constitution = 110,
            Mana = 200,
            MaxMana = 300
        };

        // Act - Serialize and deserialize
        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<PlayerData>(json, _jsonOptions);

        // Assert - All values match
        restored.Should().NotBeNull();
        restored!.Id.Should().Be(original.Id);
        restored.Name1.Should().Be(original.Name1);
        restored.Name2.Should().Be(original.Name2);
        restored.RealName.Should().Be(original.RealName);
        restored.Level.Should().Be(original.Level);
        restored.Experience.Should().Be(original.Experience);
        restored.HP.Should().Be(original.HP);
        restored.MaxHP.Should().Be(original.MaxHP);
        restored.Gold.Should().Be(original.Gold);
        restored.BankGold.Should().Be(original.BankGold);
        restored.Strength.Should().Be(original.Strength);
        restored.Defence.Should().Be(original.Defence);
        restored.Stamina.Should().Be(original.Stamina);
        restored.Agility.Should().Be(original.Agility);
        restored.Charisma.Should().Be(original.Charisma);
        restored.Dexterity.Should().Be(original.Dexterity);
        restored.Wisdom.Should().Be(original.Wisdom);
        restored.Intelligence.Should().Be(original.Intelligence);
        restored.Constitution.Should().Be(original.Constitution);
        restored.Mana.Should().Be(original.Mana);
        restored.MaxMana.Should().Be(original.MaxMana);
    }

    [Fact]
    public void PlayerData_RoundTrip_PreservesEquipmentStats()
    {
        var original = new PlayerData
        {
            Healing = 50,
            WeapPow = 250,
            ArmPow = 180,
            WeaponCursed = true,
            ArmorCursed = false,
            ShieldCursed = true
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<PlayerData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.Healing.Should().Be(original.Healing);
        restored.WeapPow.Should().Be(original.WeapPow);
        restored.ArmPow.Should().Be(original.ArmPow);
        restored.WeaponCursed.Should().Be(original.WeaponCursed);
        restored.ArmorCursed.Should().Be(original.ArmorCursed);
        restored.ShieldCursed.Should().Be(original.ShieldCursed);
    }

    [Fact]
    public void PlayerData_RoundTrip_PreservesClassAndRace()
    {
        var original = new PlayerData
        {
            Race = CharacterRace.Elf,
            Class = CharacterClass.Magician,
            Sex = 'F',
            Age = 150,
            Difficulty = DifficultyMode.Nightmare
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<PlayerData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.Race.Should().Be(CharacterRace.Elf);
        restored.Class.Should().Be(CharacterClass.Magician);
        restored.Sex.Should().Be('F');
        restored.Age.Should().Be(150);
        restored.Difficulty.Should().Be(DifficultyMode.Nightmare);
    }

    [Fact]
    public void PlayerData_RoundTrip_PreservesDailyLimits()
    {
        var original = new PlayerData
        {
            TurnCount = 500,
            TurnsRemaining = 150,
            Fights = 5,
            PFights = 3,
            TFights = 2,
            Thiefs = 4,
            Brawls = 1,
            Assa = 2,
            DarkNr = 3,
            ChivNr = 5
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<PlayerData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.TurnCount.Should().Be(original.TurnCount);
        restored.TurnsRemaining.Should().Be(original.TurnsRemaining);
        restored.Fights.Should().Be(original.Fights);
        restored.PFights.Should().Be(original.PFights);
        restored.TFights.Should().Be(original.TFights);
        restored.Thiefs.Should().Be(original.Thiefs);
        restored.Brawls.Should().Be(original.Brawls);
        restored.Assa.Should().Be(original.Assa);
        restored.DarkNr.Should().Be(original.DarkNr);
        restored.ChivNr.Should().Be(original.ChivNr);
    }

    [Fact]
    public void PlayerData_RoundTrip_PreservesPrisonState()
    {
        var original = new PlayerData
        {
            DaysInPrison = 5,
            CellDoorOpen = true,
            RescuedBy = "FriendlyNPC",
            PrisonEscapes = 2
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<PlayerData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.DaysInPrison.Should().Be(5);
        restored.CellDoorOpen.Should().BeTrue();
        restored.RescuedBy.Should().Be("FriendlyNPC");
        restored.PrisonEscapes.Should().Be(2);
    }

    #endregion

    #region NPCData Round-Trip Tests

    [Fact]
    public void NPCData_RoundTrip_PreservesAllFields()
    {
        var original = new NPCData
        {
            Id = "npc-merchant-001",
            Name = "Merchant Bob",
            Archetype = "merchant",
            HP = 100,
            MaxHP = 100,
            BaseMaxHP = 100,
            BaseMaxMana = 50,
            Level = 15,
            Strength = 50,
            Defence = 30,
            Gold = 5000,
            IsDead = false,
            Location = "MainStreet"
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<NPCData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.Id.Should().Be(original.Id);
        restored.Name.Should().Be(original.Name);
        restored.Archetype.Should().Be(original.Archetype);
        restored.HP.Should().Be(original.HP);
        restored.MaxHP.Should().Be(original.MaxHP);
        restored.BaseMaxHP.Should().Be(original.BaseMaxHP);
        restored.BaseMaxMana.Should().Be(original.BaseMaxMana);
        restored.Level.Should().Be(original.Level);
        restored.Strength.Should().Be(original.Strength);
        restored.Defence.Should().Be(original.Defence);
        restored.Gold.Should().Be(original.Gold);
        restored.IsDead.Should().Be(original.IsDead);
        restored.Location.Should().Be(original.Location);
    }

    [Fact]
    public void NPCData_RoundTrip_PreservesDeadState()
    {
        // This is critical - IsDead must round-trip correctly
        var original = new NPCData
        {
            Id = "dead-npc",
            Name = "DeadGuy",
            IsDead = true,
            HP = 0,
            MaxHP = 100
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<NPCData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.IsDead.Should().BeTrue("IsDead flag must be preserved");
        restored.HP.Should().Be(0);
    }

    [Fact]
    public void NPCData_RoundTrip_PreservesBaseStats()
    {
        // BaseMaxHP/BaseMaxMana bug was a real issue - ensure they round-trip
        var original = new NPCData
        {
            Id = "test-npc",
            MaxHP = 150,
            BaseMaxHP = 100,  // Base before equipment
            MaxMana = 75,
            BaseMaxMana = 50
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<NPCData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.BaseMaxHP.Should().Be(100, "BaseMaxHP must be preserved for RecalculateStats");
        restored.BaseMaxMana.Should().Be(50, "BaseMaxMana must be preserved for RecalculateStats");
    }

    #endregion

    #region StorySystemsData Round-Trip Tests

    [Fact]
    public void StorySystemsData_RoundTrip_PreservesSeals()
    {
        var original = new StorySystemsData
        {
            CollectedSeals = new List<int> { 15, 30, 45 },
            AwakeningLevel = 3,
            CollectedFragments = new List<int> { 1, 2, 3, 4 },
            ExperiencedMoments = new List<int> { 1, 5, 10 }
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<StorySystemsData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.CollectedSeals.Should().BeEquivalentTo(new[] { 15, 30, 45 });
        restored.AwakeningLevel.Should().Be(3);
        restored.CollectedFragments.Should().BeEquivalentTo(new[] { 1, 2, 3, 4 });
        restored.ExperiencedMoments.Should().BeEquivalentTo(new[] { 1, 5, 10 });
    }

    [Fact]
    public void StorySystemsData_RoundTrip_PreservesGriefState()
    {
        var original = new StorySystemsData
        {
            GriefStage = 2,
            GriefDaysRemaining = 5,
            GriefCompanionName = "Lyris",
            ActiveGriefs = new List<GriefStateSaveData>
            {
                new GriefStateSaveData
                {
                    CompanionName = "Kael",
                    CurrentStage = 3,
                    StageStartDay = 5,
                    GriefStartDay = 1,
                    ResurrectionAttempts = 1,
                    IsComplete = false
                }
            }
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<StorySystemsData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.GriefStage.Should().Be(2);
        restored.GriefDaysRemaining.Should().Be(5);
        restored.GriefCompanionName.Should().Be("Lyris");
        restored.ActiveGriefs.Should().HaveCount(1);
        restored.ActiveGriefs[0].CompanionName.Should().Be("Kael");
        restored.ActiveGriefs[0].CurrentStage.Should().Be(3);
    }

    [Fact]
    public void StorySystemsData_RoundTrip_PreservesStoryFlags()
    {
        var original = new StorySystemsData
        {
            StoryFlags = new Dictionary<string, bool>
            {
                { "met_king", true },
                { "defeated_boss_25", true },
                { "married", false },
                { "betrayed_ally", true }
            },
            CurrentCycle = 3
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<StorySystemsData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.StoryFlags.Should().ContainKey("met_king");
        restored.StoryFlags["met_king"].Should().BeTrue();
        restored.StoryFlags["married"].Should().BeFalse();
        restored.StoryFlags["betrayed_ally"].Should().BeTrue();
        restored.CurrentCycle.Should().Be(3);
    }

    [Fact]
    public void StorySystemsData_RoundTrip_PreservesChildren()
    {
        var original = new StorySystemsData
        {
            Children = new List<ChildData>
            {
                new ChildData
                {
                    Name = "ChildOne",
                    Age = 5,
                    Father = "PlayerName",
                    Mother = "SpouseName",
                    FatherID = "player-123",
                    MotherID = "npc-spouse-456"
                },
                new ChildData
                {
                    Name = "ChildTwo",
                    Age = 3,
                    Father = "PlayerName",
                    Mother = "SpouseName"
                }
            }
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<StorySystemsData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.Children.Should().HaveCount(2);
        restored.Children[0].Name.Should().Be("ChildOne");
        restored.Children[0].Age.Should().Be(5);
        restored.Children[0].FatherID.Should().Be("player-123");
        restored.Children[1].Name.Should().Be("ChildTwo");
    }

    #endregion

    #region Edge Cases and Boundary Tests

    [Fact]
    public void PlayerData_RoundTrip_HandlesLargeNumbers()
    {
        // Test near-max values that might overflow
        var original = new PlayerData
        {
            Experience = long.MaxValue - 1000,
            Gold = long.MaxValue / 2,
            BankGold = long.MaxValue / 3
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<PlayerData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.Experience.Should().Be(original.Experience);
        restored.Gold.Should().Be(original.Gold);
        restored.BankGold.Should().Be(original.BankGold);
    }

    [Fact]
    public void PlayerData_RoundTrip_HandlesZeroValues()
    {
        var original = new PlayerData
        {
            Level = 1,
            Experience = 0,
            HP = 0,  // Dead player
            MaxHP = 100,
            Gold = 0,
            Mana = 0
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<PlayerData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.Experience.Should().Be(0);
        restored.HP.Should().Be(0);
        restored.Gold.Should().Be(0);
        restored.Mana.Should().Be(0);
    }

    [Fact]
    public void PlayerData_RoundTrip_HandlesEmptyStrings()
    {
        var original = new PlayerData
        {
            Name1 = "",
            Name2 = "",
            RealName = "",
            CurrentLocation = "",
            RescuedBy = ""
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<PlayerData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.Name1.Should().BeEmpty();
        restored.Name2.Should().BeEmpty();
        restored.RealName.Should().BeEmpty();
        restored.CurrentLocation.Should().BeEmpty();
        restored.RescuedBy.Should().BeEmpty();
    }

    [Fact]
    public void PlayerData_RoundTrip_HandlesSpecialCharactersInNames()
    {
        var original = new PlayerData
        {
            Name1 = "Player's \"Name\"",
            Name2 = "The \\Slayer/",
            RealName = "日本語テスト"  // Unicode characters
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<PlayerData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.Name1.Should().Be("Player's \"Name\"");
        restored.Name2.Should().Be("The \\Slayer/");
        restored.RealName.Should().Be("日本語テスト");
    }

    [Fact]
    public void StorySystemsData_RoundTrip_HandlesEmptyLists()
    {
        var original = new StorySystemsData
        {
            CollectedSeals = new List<int>(),
            CollectedFragments = new List<int>(),
            Children = new List<ChildData>(),
            ActiveGriefs = new List<GriefStateSaveData>(),
            StoryFlags = new Dictionary<string, bool>()
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<StorySystemsData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.CollectedSeals.Should().BeEmpty();
        restored.CollectedFragments.Should().BeEmpty();
        restored.Children.Should().BeEmpty();
        restored.ActiveGriefs.Should().BeEmpty();
        restored.StoryFlags.Should().BeEmpty();
    }

    #endregion

    #region SaveGameData Full Round-Trip

    [Fact]
    public void SaveGameData_RoundTrip_PreservesMetadata()
    {
        var saveTime = DateTime.Now;
        var resetTime = DateTime.Now.AddHours(-2);

        var original = new SaveGameData
        {
            Version = 15,
            SaveTime = saveTime,
            LastDailyReset = resetTime,
            CurrentDay = 42,
            DailyCycleMode = DailyCycleMode.RealTime24Hour
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<SaveGameData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.Version.Should().Be(15);
        restored.CurrentDay.Should().Be(42);
        restored.DailyCycleMode.Should().Be(DailyCycleMode.RealTime24Hour);
        // DateTime comparison with tolerance for serialization
        restored.SaveTime.Should().BeCloseTo(saveTime, TimeSpan.FromSeconds(1));
        restored.LastDailyReset.Should().BeCloseTo(resetTime, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SaveGameData_RoundTrip_PreservesCompleteState()
    {
        // Create a comprehensive save with multiple systems populated
        var original = new SaveGameData
        {
            Version = 15,
            CurrentDay = 100,
            Player = new PlayerData
            {
                Name2 = "TestHero",
                Level = 50,
                HP = 500,
                MaxHP = 500,
                Gold = 100000
            },
            NPCs = new List<NPCData>
            {
                new NPCData { Id = "npc1", Name = "Guard", Level = 10 },
                new NPCData { Id = "npc2", Name = "Merchant", Level = 5 }
            },
            StorySystems = new StorySystemsData
            {
                CollectedSeals = new List<int> { 15, 30 },
                CurrentCycle = 2,
                StoryFlags = new Dictionary<string, bool> { { "flag1", true } }
            }
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<SaveGameData>(json, _jsonOptions);

        restored.Should().NotBeNull();
        restored!.Player.Name2.Should().Be("TestHero");
        restored.Player.Level.Should().Be(50);
        restored.NPCs.Should().HaveCount(2);
        restored.NPCs[0].Name.Should().Be("Guard");
        restored.StorySystems.CollectedSeals.Should().HaveCount(2);
        restored.StorySystems.StoryFlags.Should().ContainKey("flag1");
    }

    #endregion

    #region Real Bug Regression Tests

    [Fact]
    public void NPCData_BaseMaxHP_MustNotBeZero_AfterRoundTrip()
    {
        // Regression test for bug: BaseMaxHP was not saved, causing
        // RecalculateStats to set HP to 0 or negative after load
        var original = new NPCData
        {
            Id = "important-npc",
            HP = 100,
            MaxHP = 150,
            BaseMaxHP = 100,  // This MUST be preserved
            BaseMaxMana = 50
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<NPCData>(json, _jsonOptions);

        restored!.BaseMaxHP.Should().Be(100,
            "BaseMaxHP=0 would cause negative HP after RecalculateStats");
        restored.BaseMaxMana.Should().Be(50);
    }

    [Fact]
    public void StorySystemsData_CollectedSeals_MustPreserveOrder()
    {
        // Seals must be in order for floor progression checks
        var original = new StorySystemsData
        {
            CollectedSeals = new List<int> { 15, 30, 45, 60, 80, 99 }
        };

        var json = JsonSerializer.Serialize(original, _jsonOptions);
        var restored = JsonSerializer.Deserialize<StorySystemsData>(json, _jsonOptions);

        restored!.CollectedSeals.Should().ContainInOrder(15, 30, 45, 60, 80, 99);
    }

    [Fact]
    public void PlayerData_AllEnumValues_RoundTripCorrectly()
    {
        // Test all enum values to ensure serialization handles them
        foreach (CharacterClass charClass in Enum.GetValues(typeof(CharacterClass)))
        {
            foreach (CharacterRace race in Enum.GetValues(typeof(CharacterRace)))
            {
                var original = new PlayerData { Class = charClass, Race = race };
                var json = JsonSerializer.Serialize(original, _jsonOptions);
                var restored = JsonSerializer.Deserialize<PlayerData>(json, _jsonOptions);

                restored!.Class.Should().Be(charClass, $"Class {charClass} failed round-trip");
                restored.Race.Should().Be(race, $"Race {race} failed round-trip");
            }
        }
    }

    #endregion
}
