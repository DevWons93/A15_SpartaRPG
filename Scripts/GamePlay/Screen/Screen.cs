﻿
using System.Runtime.InteropServices;

namespace TextRPG
{

    // 각종 스크린 클래스의 부모클래스
    public  abstract class Screen
    {
        protected ItemDataManager dm;
        protected GameManager gm;        

        protected static int playerInput;
        
        public Screen()
        {
            dm = ItemDataManager.instance;
            gm = GameManager.instance;               
        }

        // 5.1 J => 장비 추가로 인한 리팩토링
        // 인벤토리 아이템 텍스트 출력
        protected void InventoryItemText(EquipItem equipItem)
        {
            string equip = equipItem.IsEquip ? "[E]" : "";

            Console.Write($"{equip}{equipItem.ItemName} ({equipItem.GetEquipItemClassName()}) ");

            equipItem.GetItemRankName();

            Console.Write("\t| ");

            if (equipItem.AtkValue != 0)
            {
                Console.Write($"공격력 {equipItem.AtkValue} ");
            }

            if(equipItem.DefValue != 0)
            {
                Console.Write($"방어력 {equipItem.DefValue} ");
            }

            Console.WriteLine($"|\t{equipItem.Desc}");            
        }


        // 플레이어의 행동 텍스트 출력
        protected void MyActionText()
        {
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.Write(">> ");            
        }

        protected void SystemMessageText(EMessageType messageType)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red; //빨강: 경고

            switch (messageType)
            {
                case EMessageType.DEFAULT:
                    return;
                case EMessageType.ERROR:
                    
                    Console.WriteLine("잘못된 입력입니다");
                    
                    break;
                case EMessageType.OTHERCLASSITEM:
                    Console.WriteLine("현재 직업에 맞지 않는 아이템입니다.");
                    break;
                case EMessageType.MANALESS:
                    Console.WriteLine("마나가 부족합니다.");
                    break;
                case EMessageType.BUYITEM:
                    Console.ForegroundColor = ConsoleColor.Yellow; //노랑: 알림.
                    Console.WriteLine("아이템을 구매했습니다.");
                    break;
                case EMessageType.SELL:
                    Console.ForegroundColor = ConsoleColor.Yellow; //노랑: 알림.
                    Console.WriteLine("아이템을 판매했습니다.");
                    break;
                case EMessageType.GOLD:
                    Console.WriteLine("Gold가 부족합니다.");
                    break;
                case EMessageType.ALREADYBUYITEM:
                    Console.WriteLine("이미 구매한 장비입니다.");
                    break;
                case EMessageType.SHOPRESET:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("상점 장비 아이템을 초기화했습니다.");
                    break;
                case EMessageType.SHOPRESETFAIL:
                    Console.WriteLine("모든 장비를 다 구매하셨습니다.");
                    break;
                case EMessageType.FULLCONDITION:
                    Console.WriteLine("포션을 사용할 수 없습니다. (체력 또는 마나가 이미 최대입니다)");
                    break;
            }
            Console.ResetColor();
            Thread.Sleep(750);
        }

        protected void PrintLogo()
        {
            Console.Clear();
            //generated by
            //https://textkool.com/en/ascii-art-generator?hl=default&vl=default&font=Slant&text=sparta%0ADungeon
            Console.WriteLine("================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("                           __               ");
            Console.WriteLine("    _________  ____ ______/ /_____ _        ");
            Console.WriteLine("    / ___/ __ \\/ __ `/ ___/ __/ __ `/        ");
            Console.WriteLine("   (__  ) /_/ / /_/ / /  / /_/ /_/ /         ");
            Console.WriteLine("  /____/ .___/\\__,_/_/   \\__/\\__,_/          ");
            Console.WriteLine("      ///_                                   ");
            Console.WriteLine("     / __ \\__  ______  ____ ____  ____  ____ ");
            Console.WriteLine("    / / / / / / / __ \\/ __ `/ _ \\/ __ \\/ __ \\");
            Console.WriteLine("   / /_/ / /_/ / / / / /_/ /  __/ /_/ / / / /");
            Console.WriteLine("  /_____/\\__,_/_/ /_/\\__, /\\___/\\____/_/ /_/ ");
            Console.WriteLine("                    /____/                   \n");
            Console.ResetColor();
            Console.WriteLine("================================================");
            Console.WriteLine("\n");
        }

        protected void PrintName(string name)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(name);
            Console.ResetColor();
        }

        protected void PrintTitle(string title)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"§ {title} §");
            Console.ResetColor();
        }

        protected void PrintNotice(string notice)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{notice}");
            Console.ResetColor();
        }

        public abstract void ScreenOn();
    }
}
