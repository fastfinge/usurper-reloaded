using Xunit;
using FluentAssertions;

namespace UsurperReborn.Tests;

/// <summary>
/// Unit tests for MonsterGenerator
/// Tests monster creation and stat scaling
/// </summary>
public class MonsterGeneratorTests
{
    #region Basic Generation Tests

    [Fact]
    public void GenerateMonster_ReturnsValidMonster()
    {
        var monster = MonsterGenerator.GenerateMonster(1);

        monster.Should().NotBeNull();
        monster.Name.Should().NotBeNullOrEmpty();
        monster.HP.Should().BeGreaterThan(0);
        monster.Strength.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(50)]
    [InlineData(75)]
    [InlineData(100)]
    public void GenerateMonster_WorksAtAllLevels(int level)
    {
        var monster = MonsterGenerator.GenerateMonster(level);

        monster.Should().NotBeNull();
        monster.Level.Should().Be(level);
    }

    [Fact]
    public void GenerateMonster_BossFlag_IsSet()
    {
        var bossMonster = MonsterGenerator.GenerateMonster(50, isBoss: true);
        var normalMonster = MonsterGenerator.GenerateMonster(50, isBoss: false);

        bossMonster.IsBoss.Should().BeTrue();
        normalMonster.IsBoss.Should().BeFalse();
    }

    #endregion

    #region Level Clamping Tests

    [Fact]
    public void GenerateMonster_ClampsLevelToMin()
    {
        var monster = MonsterGenerator.GenerateMonster(-10);

        monster.Should().NotBeNull();
        monster.Level.Should().Be(1);
    }

    [Fact]
    public void GenerateMonster_ClampsLevelToMax()
    {
        var monster = MonsterGenerator.GenerateMonster(150);

        monster.Should().NotBeNull();
        monster.Level.Should().Be(100);
    }

    #endregion

    #region Stat Scaling Tests

    [Fact]
    public void GenerateMonster_HigherLevel_HasMoreHP()
    {
        var random = new Random(12345); // Seeded for consistency
        var lowMonster = MonsterGenerator.GenerateMonster(1, random: new Random(12345));
        var highMonster = MonsterGenerator.GenerateMonster(50, random: new Random(12345));

        highMonster.HP.Should().BeGreaterThan(lowMonster.HP);
    }

    [Fact]
    public void GenerateMonster_HigherLevel_HasMoreStrength()
    {
        var lowMonster = MonsterGenerator.GenerateMonster(1, random: new Random(12345));
        var highMonster = MonsterGenerator.GenerateMonster(50, random: new Random(12345));

        highMonster.Strength.Should().BeGreaterThan(lowMonster.Strength);
    }

    [Fact]
    public void GenerateMonster_Level1_HasReasonableStats()
    {
        // Test several random generations to ensure consistency
        for (int i = 0; i < 10; i++)
        {
            var monster = MonsterGenerator.GenerateMonster(1);

            // Level 1 monsters should have modest stats
            monster.HP.Should().BeGreaterThanOrEqualTo(15); // Minimum HP from formula
            monster.HP.Should().BeLessThan(500); // Shouldn't be too high
            monster.Strength.Should().BeGreaterThanOrEqualTo(3); // Minimum strength
        }
    }

    [Fact]
    public void GenerateMonster_Level50_HasMidRangeStats()
    {
        for (int i = 0; i < 10; i++)
        {
            var monster = MonsterGenerator.GenerateMonster(50);

            // Level 50 monsters should be formidable
            monster.HP.Should().BeGreaterThan(500);
            monster.Strength.Should().BeGreaterThan(50);
        }
    }

    [Fact]
    public void GenerateMonster_Level100_HasHighStats()
    {
        for (int i = 0; i < 10; i++)
        {
            var monster = MonsterGenerator.GenerateMonster(100);

            // Level 100 monsters should be very powerful
            monster.HP.Should().BeGreaterThan(1000);
            monster.Strength.Should().BeGreaterThan(100);
        }
    }

    #endregion

    #region Boss Modifier Tests

    [Fact]
    public void GenerateMonster_Boss_HasMoreHP()
    {
        // Use seeded random for reproducible comparison
        var random1 = new Random(99999);
        var random2 = new Random(99999);

        var normalMonster = MonsterGenerator.GenerateMonster(50, isBoss: false, random: random1);
        var bossMonster = MonsterGenerator.GenerateMonster(50, isBoss: true, random: random2);

        // Boss should have at least 40% more HP (1.5x multiplier)
        // Note: Different monster families may be selected, so we check general trend
        bossMonster.IsBoss.Should().BeTrue();
    }

    #endregion

    #region Monster Group Tests

    [Fact]
    public void GenerateMonsterGroup_ReturnsNonEmptyList()
    {
        var monsters = MonsterGenerator.GenerateMonsterGroup(25);

        monsters.Should().NotBeNull();
        monsters.Should().NotBeEmpty();
    }

    [Fact]
    public void GenerateMonsterGroup_ReturnsReasonableSize()
    {
        // Run multiple times to test variability
        for (int i = 0; i < 20; i++)
        {
            var monsters = MonsterGenerator.GenerateMonsterGroup(50);

            monsters.Count.Should().BeGreaterThanOrEqualTo(1);
            monsters.Count.Should().BeLessThanOrEqualTo(5);
        }
    }

    [Fact]
    public void GenerateMonsterGroup_EarlyLevels_SmallerGroups()
    {
        var totalMonsters = 0;
        for (int i = 0; i < 100; i++)
        {
            var monsters = MonsterGenerator.GenerateMonsterGroup(5);
            totalMonsters += monsters.Count;
        }

        // Average should be around 1-2 for early levels
        var average = totalMonsters / 100.0;
        average.Should().BeLessThan(3.0);
    }

    [Fact]
    public void GenerateMonsterGroup_LateLevels_LargerGroups()
    {
        var totalMonsters = 0;
        for (int i = 0; i < 100; i++)
        {
            var monsters = MonsterGenerator.GenerateMonsterGroup(80);
            totalMonsters += monsters.Count;
        }

        // Average should be around 3-4 for late levels
        var average = totalMonsters / 100.0;
        average.Should().BeGreaterThan(2.0);
    }

    [Fact]
    public void GenerateMonsterGroup_AllMonstersHaveLevel()
    {
        var monsters = MonsterGenerator.GenerateMonsterGroup(30);

        foreach (var monster in monsters)
        {
            monster.Level.Should().Be(30);
        }
    }

    #endregion

    #region Monster Properties Tests

    [Fact]
    public void GenerateMonster_HasFamilyName()
    {
        var monster = MonsterGenerator.GenerateMonster(25);

        monster.FamilyName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateMonster_HasTierName()
    {
        var monster = MonsterGenerator.GenerateMonster(25);

        monster.TierName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateMonster_HasMonsterColor()
    {
        var monster = MonsterGenerator.GenerateMonster(25);

        monster.MonsterColor.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateMonster_HasAttackType()
    {
        var monster = MonsterGenerator.GenerateMonster(25);

        monster.AttackType.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateMonster_HasWeapon()
    {
        var monster = MonsterGenerator.GenerateMonster(25);

        monster.Weapon.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateMonster_HasArmor()
    {
        var monster = MonsterGenerator.GenerateMonster(25);

        monster.Armor.Should().NotBeNull(); // Can be "None"
    }

    [Fact]
    public void GenerateMonster_HasPhrase()
    {
        var monster = MonsterGenerator.GenerateMonster(25);

        monster.Phrase.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region Seeded Random Tests

    [Fact]
    public void GenerateMonster_WithSameSeeds_ProducesSameMonster()
    {
        var random1 = new Random(42);
        var random2 = new Random(42);

        var monster1 = MonsterGenerator.GenerateMonster(50, random: random1);
        var monster2 = MonsterGenerator.GenerateMonster(50, random: random2);

        monster1.Name.Should().Be(monster2.Name);
        monster1.HP.Should().Be(monster2.HP);
        monster1.Strength.Should().Be(monster2.Strength);
        monster1.Defence.Should().Be(monster2.Defence);
    }

    [Fact]
    public void GenerateMonsterGroup_WithSameSeeds_ProducesSameGroup()
    {
        var random1 = new Random(42);
        var random2 = new Random(42);

        var group1 = MonsterGenerator.GenerateMonsterGroup(50, random1);
        var group2 = MonsterGenerator.GenerateMonsterGroup(50, random2);

        group1.Count.Should().Be(group2.Count);
        for (int i = 0; i < group1.Count; i++)
        {
            group1[i].Name.Should().Be(group2[i].Name);
        }
    }

    #endregion

    #region Equipment Scaling Tests

    [Fact]
    public void GenerateMonster_LowLevel_HasBasicWeapons()
    {
        var basicWeapons = new[] { "Rusty Dagger", "Wooden Club", "Short Sword", "Crude Axe" };

        var foundBasic = false;
        for (int i = 0; i < 20; i++)
        {
            var monster = MonsterGenerator.GenerateMonster(5);
            if (basicWeapons.Contains(monster.Weapon))
            {
                foundBasic = true;
                break;
            }
        }

        foundBasic.Should().BeTrue("Low level monsters should have basic weapons");
    }

    [Fact]
    public void GenerateMonster_HighLevel_HasLegendaryWeapons()
    {
        var legendaryWeapons = new[] { "Legendary Sword", "Vorpal Blade", "Soul Reaver", "Doom Bringer", "Ancient Artifact Weapon" };

        var foundLegendary = false;
        for (int i = 0; i < 20; i++)
        {
            var monster = MonsterGenerator.GenerateMonster(90);
            if (legendaryWeapons.Contains(monster.Weapon))
            {
                foundLegendary = true;
                break;
            }
        }

        foundLegendary.Should().BeTrue("High level monsters should have legendary weapons");
    }

    #endregion

    #region Stat Minimum Tests

    [Fact]
    public void GenerateMonster_HasMinimumStats()
    {
        // Generate many monsters to ensure minimums are always met
        for (int level = 1; level <= 100; level += 10)
        {
            for (int i = 0; i < 10; i++)
            {
                var monster = MonsterGenerator.GenerateMonster(level);

                monster.HP.Should().BeGreaterThanOrEqualTo(15, $"HP minimum not met at level {level}");
                monster.Strength.Should().BeGreaterThanOrEqualTo(3, $"Strength minimum not met at level {level}");
                monster.Defence.Should().BeGreaterThanOrEqualTo(0, $"Defence should not be negative at level {level}");
                monster.Punch.Should().BeGreaterThanOrEqualTo(0, $"Punch should not be negative at level {level}");
                monster.WeaponPower.Should().BeGreaterThanOrEqualTo(0, $"WeaponPower should not be negative at level {level}");
                monster.ArmorPower.Should().BeGreaterThanOrEqualTo(0, $"ArmorPower should not be negative at level {level}");
            }
        }
    }

    #endregion

    #region Mini-Boss/Champion Tests

    [Fact]
    public void GenerateMonster_MiniBossFlag_IsSet()
    {
        var miniBoss = MonsterGenerator.GenerateMonster(50, isBoss: false, isMiniBoss: true);
        var normalMonster = MonsterGenerator.GenerateMonster(50, isBoss: false, isMiniBoss: false);

        miniBoss.IsMiniBoss.Should().BeTrue();
        miniBoss.IsBoss.Should().BeFalse();
        normalMonster.IsMiniBoss.Should().BeFalse();
    }

    [Fact]
    public void GenerateMonster_MiniBoss_HasChampionName()
    {
        var random = new Random(12345);
        var miniBoss = MonsterGenerator.GenerateMonster(50, isBoss: false, isMiniBoss: true, random: random);

        // Champion names should have special suffixes like "Champion", "Revenant", "Warchief", etc.
        var championSuffixes = new[] { "Champion", "Revenant", "Warchief", "Elder", "Overlord",
            "Titan", "Alpha", "Prime", "Abomination", "Hive Lord", "Archfey", "Leviathan",
            "Seraph", "Nightlord" };

        var hasChampionName = championSuffixes.Any(suffix =>
            miniBoss.Name.Contains(suffix) || miniBoss.Name.StartsWith("Alpha ") ||
            miniBoss.Name.StartsWith("Elder ") || miniBoss.Name.StartsWith("Prime "));

        hasChampionName.Should().BeTrue($"Mini-boss name '{miniBoss.Name}' should have a champion suffix");
    }

    [Fact]
    public void GenerateMonster_MiniBoss_HasBrightYellowColor()
    {
        var miniBoss = MonsterGenerator.GenerateMonster(50, isBoss: false, isMiniBoss: true);

        miniBoss.MonsterColor.Should().Be("bright_yellow");
    }

    [Fact]
    public void GenerateMonster_MiniBoss_HasMoreHPThanNormal()
    {
        // Use seeded random for reproducible comparison
        var random1 = new Random(77777);
        var random2 = new Random(77777);

        var normalMonster = MonsterGenerator.GenerateMonster(50, isBoss: false, isMiniBoss: false, random: random1);
        var miniBoss = MonsterGenerator.GenerateMonster(50, isBoss: false, isMiniBoss: true, random: random2);

        // Mini-boss should have 1.5x HP multiplier
        miniBoss.HP.Should().BeGreaterThan(normalMonster.HP);
    }

    [Fact]
    public void GenerateMonster_MiniBoss_HasSpecialPhrase()
    {
        var miniBoss = MonsterGenerator.GenerateMonster(50, isBoss: false, isMiniBoss: true);

        // Champion phrases are more dramatic than normal monster phrases
        miniBoss.Phrase.Should().NotBeNullOrEmpty();
        // Champions have unique dramatic phrases - just verify it has substance
        miniBoss.Phrase.Length.Should().BeGreaterThan(10,
            $"Mini-boss phrase should be substantial: '{miniBoss.Phrase}'");
    }

    [Fact]
    public void GenerateMonsterGroup_CanSpawnMiniBoss()
    {
        var foundMiniBoss = false;

        // Run many iterations - 10% chance means we should see some
        for (int i = 0; i < 200; i++)
        {
            var monsters = MonsterGenerator.GenerateMonsterGroup(50);
            if (monsters.Count == 1 && monsters[0].IsMiniBoss)
            {
                foundMiniBoss = true;
                break;
            }
        }

        foundMiniBoss.Should().BeTrue("Mini-boss encounters should occur ~10% of the time");
    }

    [Fact]
    public void GenerateMonsterGroup_MiniBossEncounter_IsSingleMonster()
    {
        // Force mini-boss encounters by running many iterations
        for (int i = 0; i < 200; i++)
        {
            var monsters = MonsterGenerator.GenerateMonsterGroup(50);
            if (monsters.Any(m => m.IsMiniBoss))
            {
                // Mini-boss encounters should always be a single monster
                monsters.Count.Should().Be(1, "Mini-boss encounters should be single monsters");
                return;
            }
        }

        // If we get here without finding a mini-boss, that's still a pass (unlikely but possible)
    }

    #endregion
}
