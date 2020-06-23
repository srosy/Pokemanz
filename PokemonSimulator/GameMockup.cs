﻿using PokeAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonSimulator
{
    class GameMockup
    {
        static TypeWeaknessMap twm = new TypeWeaknessMap();
        public static void Main_Other(string[] args)
        {
            //Tie this to the stuff to get real things.
            UserAuthAndLogin login = new UserAuthAndLogin();
            TrainerLineUp yourLineUp = new TrainerLineUp(login.UserID, login.TrainerName, login.Connection.myConnection);
            Trainer you = yourLineUp.GhostTrainer;
            //Trainer you = new Trainer();
            Trainer enemy = new Trainer();
            do
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Battle Begin!");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Choose your pokemon: (1 - 6) " + string.Join(", ", you.Pokemon.Select(x => x.Species)));
                Pokemon yours;
                do
                {
                    int.TryParse(Console.ReadLine(), out int tryParse);
                    tryParse -= 1;
                    if (tryParse >= 0 && tryParse < 6)
                    {
                        yours = you.Pokemon[tryParse];
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Hey! that's not a valid number. Pick a pokemon (1 - 6)");
                    }
                } while (true);
                Pokemon enemies = enemy.Pokemon[Grand.rand.Next(0, enemy.Pokemon.Count)];
                void PrintYou(string pre, string text)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(pre);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(text);
                }
                void PrintEnemy(string pre, string text)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(pre);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(text);
                }
                void Attack(Pokemon attack, Pokemon defend, int moveIndex, bool isEnemyAttacking)
                {
                    PrintEnemy($"{attack.Species} ", $"used {attack.Moves[moveIndex].Name}!");
                    int damage = attack.Moves[moveIndex].Damage;
                    //double modifier = PokemonType.CalculateDamageMultiplier(attack.Type, defend.Type);
                    double modifier = 1d;
                    for (int i = 0; i < attack.Types.Count; i++)
                    {
                        for (int j = 0; j < defend.Types.Count; j++)
                        {
                            modifier *= PokemonType.CalculateDamageMultiplier(attack.Types[i], defend.Types[j]);
                        }
                    }
                    int final = 0;
                    if (attack.Moves[moveIndex].Category == "special")
                    {
                        final = (int)(((double)damage * (double)modifier * (double)attack.SpecialAttack) / (double)defend.SpecialDefense);
                    }
                    else
                    {
                        final = (int)(((double)damage * (double)modifier * (double)attack.Attack) / (double)defend.Defense);
                    }
                    if (modifier > 1)
                    {
                        if (isEnemyAttacking)
                        {
                            PrintEnemy("", $"Your enemy's pokemon's attack was super effective! It hit for {final} damage!");
                        }
                        else
                        {
                            PrintYou("", $"Your pokemon's attack was super effective! It hit for {final} damage!");
                        }
                    }
                    else
                    {
                        if (isEnemyAttacking)
                        {
                            PrintEnemy("", $"Your enemy's pokemon's attack hit for {final} damage.");
                        }
                        else
                        {
                            PrintYou("", $"Your pokemon's attack hit for {final} damage!");
                        }
                    }
                    defend.ModifyHealth(-final);
                }
                PrintYou("You: ", $" Go {yours.Species}!");
                PrintEnemy("Enemy: ", $" Go {enemies.Species}!");
                bool gaming = true;
                while (gaming)
                {
                    Console.WriteLine("Select move: (1 - 4) " + string.Join(", ", yours.Moves.Select(x => x.Name)) + " or switch pokemon (5)");
                    int yourMovePick = 0;
                    do
                    {
                        if (int.TryParse(Console.ReadLine(), out int tryParse))
                        {
                            if (tryParse < 6 && tryParse > 0)
                            {
                                yourMovePick = tryParse - 1;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Hey! Pick a move (1 - 4) or switch pokemon (5).");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Hey! Thats not a number, pick a move (1 - 4) or switch pokemon (5).");
                        }
                    } while (true);
                    int enemiesMovePick = Grand.rand.Next(0, 4);
                    if (yourMovePick != 4)
                    {
                        //you and your enemy hit eachother based on speed.
                        if (enemies.Speed > yours.Speed)
                        {
                            Attack(enemies, yours, enemiesMovePick, true);
                            if (yours.IsAlive)
                            {
                                Attack(yours, enemies, yourMovePick, false);
                            }
                        }
                        else if (yours.Speed > enemies.Speed)
                        {
                            Attack(yours, enemies, enemiesMovePick, false);
                            if (enemies.IsAlive)
                            {
                                Attack(enemies, yours, yourMovePick, true);
                            }
                        }
                        else
                        {
                            //if speeds are same pick one randomly
                            switch (Grand.rand.Next(0, 2))
                            {
                                case 0:
                                    {
                                        Attack(enemies, yours, enemiesMovePick, true);
                                        if (yours.IsAlive)
                                        {
                                            Attack(yours, enemies, yourMovePick, false);
                                        }
                                    }
                                    break;
                                case 1:
                                    {
                                        Attack(yours, enemies, enemiesMovePick, false);
                                        if (enemies.IsAlive)
                                        {
                                            Attack(enemies, yours, yourMovePick, true);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    else if (yourMovePick == 4)
                    {
                        Console.WriteLine("Choose your pokemon: (1 - 6) " + string.Join(", ", you.Pokemon.Select(x => x.Species + " " + ((float)x.ActingHP / (float)x.BaseHP) + "%HP")));
                        int pokePick = int.Parse(Console.ReadLine()) - 1;
                        do
                        {
                            if (you.Pokemon[pokePick] == yours)
                            {
                                Console.WriteLine("That pokemon is already out! Pick a different one. (1 - 6)");
                            }
                            else if (!you.Pokemon[pokePick].IsAlive)
                            {
                                Console.WriteLine("That pokemon has been knocked out! Pick a different one. (1 - 6)");
                            }
                            else
                            {
                                yours = you.Pokemon[pokePick];
                                break;
                            }
                        } while (true);
                        //yours.ModifyHealth(-int.Parse(enemies.Moves[enemiesMovePick].Damage) * (yours.TypeWeaknesses.Count(x => x == enemies.Moves[enemiesMovePick].Type) + 1));
                        Attack(enemies, yours, enemiesMovePick, true);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                    if (!enemies.IsAlive)
                    {
                        IEnumerable<Pokemon> temp = enemy.Pokemon.Where(x => x.IsAlive);
                        enemies = temp.ElementAtOrDefault(Grand.rand.Next(0, temp.Count()));
                    }
                    if (!yours.IsAlive)
                    {
                        PrintYou($"{yours.Species} ", "was knocked out! Choose a new pokemon: (1 - 6) " + string.Join(", ", you.Pokemon.Select(x => x.Species + " " + ((float)x.ActingHP / (float)x.BaseHP) + "%HP")));
                        int pokePick = int.Parse(Console.ReadLine()) - 1;
                        do
                        {
                            if (you.Pokemon[pokePick] == yours)
                            {
                                Console.WriteLine("That pokemon is already out! Pick a different one. (1 - 6)");
                            }
                            else if (!you.Pokemon[pokePick].IsAlive)
                            {
                                Console.WriteLine("That pokemon has been knocked out! Pick a different one. (1 - 6)");
                            }
                            else
                            {
                                yours = you.Pokemon[pokePick];
                                break;
                            }
                        } while (true);
                    }
                    if (!enemy.Pokemon.Any(x => x.IsAlive))
                    {
                        PrintYou("You win! ", "Would you like to play again? (Y/N)");
                        gaming = false;
                    }
                    else if (!you.Pokemon.Any(x => x.IsAlive))
                    {
                        PrintEnemy("You lose. ", "Would you like to play again? (Y/N)");
                        gaming = false;
                    }
                }
                bool quit = false;
                do
                {
                    string reply = Console.ReadLine();
                    if (Grand.yes.IsMatch(reply))
                    {
                        gaming = true;
                        you = enemy = null;
                        yours = enemies = null;
                        quit = false;
                        Console.Clear();
                        break;
                    }
                    else if (Grand.no.IsMatch(reply))
                    {
                        gaming = false;
                        quit = true;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Hey! That's not a valid response. Say yes or no.");
                    }
                }
                while (true);
                if (quit)
                {
                    break;
                }
            }
            while (true);
        }
    }
}