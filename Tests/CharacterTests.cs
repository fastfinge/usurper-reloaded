using Xunit;
using FluentAssertions;

namespace UsurperReborn.Tests;

/// <summary>
/// Unit tests for Character class and related functionality
/// </summary>
public class CharacterTests
{
    [Fact]
    public void NewCharacter_HasDefaultValues()
    {
        var character = new Character();

        character.Level.Should().Be(1);
        character.Gold.Should().Be(0);
        character.HP.Should().Be(0);
        character.Name2.Should().BeEmpty();
    }

    [Fact]
    public void Character_CanSetBasicProperties()
    {
        var character = new Character
        {
            Name2 = "TestHero",
            Level = 10,
            Gold = 5000,
            HP = 100,
            MaxHP = 100,
            Strength = 50,
            Defence = 30
        };

        character.Name2.Should().Be("TestHero");
        character.Level.Should().Be(10);
        character.Gold.Should().Be(5000);
        character.HP.Should().Be(100);
        character.Strength.Should().Be(50);
        character.Defence.Should().Be(30);
    }

    [Theory]
    [InlineData(CharacterClass.Warrior)]
    [InlineData(CharacterClass.Cleric)]
    [InlineData(CharacterClass.Magician)]
    [InlineData(CharacterClass.Sage)]
    [InlineData(CharacterClass.Paladin)]
    [InlineData(CharacterClass.Assassin)]
    public void Character_CanSetClass(CharacterClass charClass)
    {
        var character = new Character { Class = charClass };

        character.Class.Should().Be(charClass);
    }

    [Theory]
    [InlineData(CharacterRace.Human)]
    [InlineData(CharacterRace.Elf)]
    [InlineData(CharacterRace.Dwarf)]
    [InlineData(CharacterRace.Orc)]
    [InlineData(CharacterRace.HalfElf)]
    [InlineData(CharacterRace.Troll)]
    public void Character_CanSetRace(CharacterRace race)
    {
        var character = new Character { Race = race };

        character.Race.Should().Be(race);
    }

    [Fact]
    public void Character_MaxPotions_CalculatesCorrectly()
    {
        var character = new Character { Level = 1 };
        character.MaxPotions.Should().Be(20); // 20 + (1-1) = 20

        character.Level = 10;
        character.MaxPotions.Should().Be(29); // 20 + (10-1) = 29

        character.Level = 50;
        character.MaxPotions.Should().Be(69); // 20 + (50-1) = 69
    }

    [Fact]
    public void Character_IsAlive_WhenHPGreaterThanZero()
    {
        var character = new Character { HP = 100, MaxHP = 100 };

        character.IsAlive.Should().BeTrue();

        character.HP = 0;
        character.IsAlive.Should().BeFalse();

        character.HP = -10;
        character.IsAlive.Should().BeFalse();
    }

    [Fact]
    public void Character_Statistics_InitializedByDefault()
    {
        var character = new Character();

        character.Statistics.Should().NotBeNull();
        character.Statistics.TotalMonstersKilled.Should().Be(0);
        character.Statistics.TotalDamageDealt.Should().Be(0);
    }

    [Fact]
    public void Character_CanTrackDiseases()
    {
        var character = new Character();

        character.Plague.Should().BeFalse();
        character.Blind.Should().BeFalse();
        character.Leprosy.Should().BeFalse();

        character.Plague = true;
        character.Plague.Should().BeTrue();

        // Multiple diseases can be active
        character.Blind = true;
        character.Plague.Should().BeTrue();
        character.Blind.Should().BeTrue();
    }

    [Fact]
    public void Character_BankGold_SeparateFromGold()
    {
        var character = new Character
        {
            Gold = 1000,
            BankGold = 5000
        };

        character.Gold.Should().Be(1000);
        character.BankGold.Should().Be(5000);

        // Depositing money
        character.Gold -= 500;
        character.BankGold += 500;

        character.Gold.Should().Be(500);
        character.BankGold.Should().Be(5500);
    }

    [Theory]
    [InlineData(CharacterSex.Male)]
    [InlineData(CharacterSex.Female)]
    public void Character_CanSetSex(CharacterSex sex)
    {
        var character = new Character { Sex = sex };

        character.Sex.Should().Be(sex);
    }

    [Fact]
    public void Character_ManaForSpellcasters()
    {
        var mage = new Character
        {
            Class = CharacterClass.Magician,
            Mana = 100,
            MaxMana = 150
        };

        mage.Mana.Should().Be(100);
        mage.MaxMana.Should().Be(150);

        // Casting spell costs mana
        mage.Mana -= 20;
        mage.Mana.Should().Be(80);
    }

    [Fact]
    public void Character_ChivalryAndDarkness_TrackMorality()
    {
        var character = new Character
        {
            Chivalry = 100,
            Darkness = 50
        };

        character.Chivalry.Should().Be(100);
        character.Darkness.Should().Be(50);

        // Doing good deeds
        character.Chivalry += 10;
        character.Chivalry.Should().Be(110);

        // Doing evil deeds
        character.Darkness += 20;
        character.Darkness.Should().Be(70);
    }
}
