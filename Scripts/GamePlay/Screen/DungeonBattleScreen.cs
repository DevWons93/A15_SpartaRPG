﻿using System.Collections.Generic;

namespace TextRPG
{
    public class DungeonBattleScreen : Screen
    {
        private List<Enemy> enemies;  // 여러 몬스터를 저장할 리스트


        private bool isEnd;
        private bool isWin;
        private bool returnToChooseEnemy = false; // 스킬 예외처리


        private DungeonResultScreen dungeonResultScreen;
        public DungeonBattleScreen()
        {
            dungeonResultScreen = new DungeonResultScreen();
            enemies = new List<Enemy>(); ;  // 몬스터를 저장할 리스트 초기화
        }

        public void CheckforBattle()
        {

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n정말 던전에 진입하시겠습니까? 끝을 보시거나, 죽기 전까지 탈출하실 수 없습니다.");
                Console.WriteLine();
                Console.WriteLine("1. 들어간다");
                Console.WriteLine("0. 나간다\n");

                MyActionText();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        BattleStart();
                        return;
                    default:
                        return;
                }
            }

        }
        private void AppearEnemy()
        {
            int currentDungeonLevel = gm.Player.Level; // 임시로 집어 넣음, 원래는 던전 난이도를 집어 넣어야함
            List<Enemy> originalEnemies = EnemyDataManager.instance.GetSpawnMonsters(currentDungeonLevel);  // 몬스터 데이터 매니저에서 몬스터 리스트 가져오기
            enemies = new List<Enemy>(originalEnemies.Select(e => new Enemy(e))); // 깊은 복사를 통해 리스트 복제

            foreach (var enemy in enemies)
            {
                Console.WriteLine($"{enemy.Name}가 나타났습니다!");
            }
        }

        public void BattleStart() // 전투 시작
        {
            isWin = false;
            isEnd = false;
            // 5.2 J => 전투 결과 창에서 불러올 수 있도록 함
            AppearEnemy();
            dungeonBattle(); // 전투 시작
        }

        private void dungeonBattle()
        {
            Console.Clear();

            while ((enemies.Any(e => e.Health > 0) && gm.Player.Health > 0))
            {
                BattleText();
                int targetIndex = ChooseEnemy();
                if (targetIndex == -1) continue;

                int actionResult = PlayerAction(enemies[targetIndex]);
                if (actionResult == -1) continue;

                if (isEnd)
                {
                    break;
                }


                foreach (var enemy in enemies.Where(e => e.Health > 0))
                {
                    EnemyTurn(enemy);
                }
            }

            // 5.2 j => 배틀 재시작, 로비로 가기 수정
            EDungeonResultType dungeonResultType = isWin ? EDungeonResultType.VICTORY : EDungeonResultType.RETIRE;

            if(dungeonResultScreen.DungeonResultScreenOn(dungeonResultType, EDungeonDifficulty.NORMAL))
            {
                BattleStart();
            }
        }

        private int ChooseEnemy()
        {
            while (true)
            {
                Console.WriteLine("공격할 몬스터를 선택하세요:");
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].Health > 0)
                    {
                        Console.WriteLine($"{i + 1}. {enemies[i].Name} (HP: {enemies[i].Health}/{enemies[i].MaxHealth})");
                    }
                }

                Console.Write("\n>>  ");

                string input = Console.ReadLine();

                // 입력이 공백이거나 null인 경우
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("입력이 잘못되었습니다. 다시 입력해주세요.");
                    continue;
                }

                // 숫자로 변환할 수 없는 경우
                if (!int.TryParse(input, out int selected))
                {
                    Console.WriteLine("숫자를 입력하세요.");
                    continue;
                }

                selected -= 1; // 선택한 몬스터의 인덱스를 계산

                // 유효한 선택이 아닌 경우
                if (selected < 0 || selected >= enemies.Count || enemies[selected].Health <= 0)
                {
                    Console.WriteLine("잘못된 선택입니다.");
                    continue;
                }

                return selected; // 유효한 선택일 경우 선택한 몬스터의 인덱스를 반환
            }
        }

        private int PlayerAction(Enemy enemy)
        {
            while (true)
            {
                Console.WriteLine("\n행동을 선택하세요:");
                Console.WriteLine("0. 다른 적 선택");
                Console.WriteLine("1. 기본 공격");
                Console.WriteLine("2. 스킬 사용\n");
                MyActionText();

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "0":
                        return -1;  // 다른 적을 선택하게 하기 위해 특별한 값을 반환
                    case "1":
                        PlayerTurn(enemy);
                        return 0;
                    case "2":
                        UseSkill(enemy);
                        if (returnToChooseEnemy)
                        {
                            returnToChooseEnemy = false; // 상태 초기화
                            return -1; // 다른 적을 선택하도록 플로우 변경
                        }
                        return 0;
                    default:
                        Console.WriteLine("잘못된 선택입니다.");
                        continue;
                }
            }
        }

        private void PlayerTurn(Enemy enemy)
        {
            // 선택한 몬스터의 이름을 포함하여 공격 메시지 출력
            Console.WriteLine($"{gm.Player.Name}의 {enemy.Name}를 향한 공격!");

            if (gm.Player.Health <= 0)
            {
                BattleEnd(false);
                return;
            } 

            string attackResult = gm.Player.Attack(enemy);
            Console.WriteLine(attackResult);
            Thread.Sleep(2000);

            if (enemy.Health <= 0)
            {
                BattleEnd( true);
                Console.WriteLine($"[{enemy.Name}이(가) 쓰러졌습니다.]");
            }
            else
            {
                Console.Clear();
            }
        }

        private void UseSkill(Enemy enemy)
        {
            while (true)
            {
                Console.WriteLine("사용할 스킬을 선택하세요 (0을 누르면 다른 적 선택):");
                for (int i = 0; i < gm.Player.Skills.Count; i++)
                {
                    var skill = gm.Player.Skills[i];
                    Console.WriteLine($"{i + 1}. {skill.Name} (MP: {skill.ManaCost}) - {skill.Content}");
                }

                string input = Console.ReadLine();
                if (input == "0")
                {
                    returnToChooseEnemy = true;
                    return; // 다른 적을 선택하도록 하기 위해 메서드 종료
                }

                if (!int.TryParse(input, out int selectedSkillIndex))
                {
                    Console.WriteLine("숫자로만 입력해주세요.");
                    continue;
                }

                selectedSkillIndex -= 1;
                if (selectedSkillIndex < 0 || selectedSkillIndex >= gm.Player.Skills.Count)
                {
                    Console.WriteLine("잘못된 선택입니다.");
                    continue;
                }

                SkillData selectedSkill = gm.Player.Skills[selectedSkillIndex];
                if (gm.Player.Mana < selectedSkill.ManaCost)
                {
                    Console.WriteLine("마나가 부족합니다.");
                    continue;
                }

                gm.Player.CostMana(selectedSkill.ManaCost);
                UseSelectedSkill(selectedSkill, enemy);
                break; // 스킬 사용 후 정상 종료
            }
        }

        private void UseSelectedSkill(SkillData skill, Enemy target)
        {
            if (skill.IsMultiTarget)
            {
                Console.WriteLine("다중 대상 스킬 사용 중...");

                // 첫 번째로 지정된 타겟에 스킬 적용
                Console.WriteLine($"{gm.Player.Name}의 {target.Name}을(를) 향한 공격!");
                string initialSkillResult = skill.CastSkill(gm.Player, target);
                Console.WriteLine(initialSkillResult);
                Thread.Sleep(2000);

                // 나머지 타겟들에게 스킬 적용
                int targetsHit = 1; // 첫 번째 타겟이 이미 공격받았으므로 1로 시작
                foreach (var enemy in enemies.Where(e => e.Health > 0 && e != target))
                {
                    if (targetsHit >= skill.MaxTargetCount)
                        break;

                    Console.WriteLine($"이어지는 {enemy.Name}을(를) 향한 공격!");
                    string skillResult = skill.CastSkill(gm.Player, enemy);
                    Console.WriteLine(skillResult);
                    Thread.Sleep(2000);

                    targetsHit++;
                }
            }
            else
            {
                // 단일 대상 스킬 사용
                Console.WriteLine($"{gm.Player.Name} {target.Name}을 향한 공격!");
                string skillResult = skill.CastSkill(gm.Player, target);
                Console.WriteLine(skillResult);
                Thread.Sleep(2000);
            }

            // 남은 적이 없으면 전투 승리 처리
            if (enemies.All(e => e.Health <= 0))
            {
                BattleEnd(true);
            }
            else
            {
                Console.Clear();
            }
        }


        private void EnemyTurn(Enemy enemy)
        {
            if (enemy.Health <= 0)
            {
                BattleEnd(true);
                return;
            }

            Console.WriteLine($"{enemy.Name}의 공격!");


            string attackResult = enemy.Attack(gm.Player);
            Console.WriteLine(attackResult);


            if (gm.Player.Health <= 0)
            {
                BattleEnd(false);
            }
            else
            {
                Thread.Sleep(2000);
            }
        }


        private void BattleText()
        {
            Console.Clear();
            Console.WriteLine("=== 전투 중인 몬스터 목록 ===");
            foreach (var en in enemies.Where(e => e.Health > 0))
            {
                Console.WriteLine($"몬스터: {en.Name}, HP: {en.Health}/{en.MaxHealth}, 공격력: {en.Atk}");
            }
            Console.WriteLine("=== 내 정보 ===");
            Console.WriteLine($"Lv.{gm.Player.Level} {gm.Player.Name} ({gm.Player.GetPlayerClass(gm.Player.ePlayerClass)})");
            Console.WriteLine($"HP {gm.Player.Health}/{gm.Player.MaxHealth}\n");
            Console.WriteLine($"MP {gm.Player.Mana}/{gm.Player.MaxMana}");
        }

        private void BattleEnd(bool isWin)
        {
            isEnd = true;
            this.isWin = isWin;
        }
    }
}