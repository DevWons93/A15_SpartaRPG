﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    public class CreditScreen : Screen
    {

        public override void ScreenOn()
        {
            Console.Clear();

            Console.WriteLine(" ______                  __  __  __   ");
            Thread.Sleep(200);
            Console.WriteLine("|      |.----..-----..--|  ||__||  |_ ");
            Thread.Sleep(200);
            Console.WriteLine("|   ---||   _||  -__||  _  ||  ||   _|");
            Thread.Sleep(200);
            Console.WriteLine("|______||__|  |_____||_____||__||____|");
            Console.WriteLine("\n");
            Thread.Sleep(500);

            Console.WriteLine("<< 조장: 지승도 >>");
            Thread.Sleep(500);
            Console.Write("맡은 작업: 베이스 코드   ");
            Console.Write("전투결과   ");
            Console.WriteLine("장비&소비템\n");
            Thread.Sleep(500);

            Console.WriteLine("<< 조원: 정승연 >>");
            Thread.Sleep(500);
            Console.Write("맡은 작업: 몬스터   ");
            Console.Write("퀘스트   ");
            Console.Write("콘솔UI   ");
            Console.WriteLine("크래딧\n");
            Thread.Sleep(500);

            Console.WriteLine("<< 조원: 장지원 >>");
            Thread.Sleep(500);
            Console.Write("맡은 작업: 직업   ");
            Console.Write("공격로직   ");
            Console.Write("스킬   ");
            Console.Write("아이템등급   ");
            Console.WriteLine("보스데이터\n");
            Thread.Sleep(500);

            Console.WriteLine("<< 조원: 김태형A >>");
            Thread.Sleep(500);
            Console.Write("맡은 작업: 던전   ");
            Console.Write("배틀씬   ");
            Console.Write("포션 사용   ");
            Console.WriteLine("보스전\n");
            Thread.Sleep(500);

            Console.Write("\nMade By.");
            Thread.Sleep(500);
            Console.WriteLine(" VS Community2022 - 17.9.6\n");
            Thread.Sleep(500);
            Console.WriteLine("~ Special Thanks ~");
            Thread.Sleep(500);
            Console.WriteLine("  _________                    __          ");
            Thread.Sleep(200);
            Console.WriteLine(" /   _____/__________ ________/  |______   ");
            Thread.Sleep(200);
            Console.WriteLine(" \\_____  \\\\____ \\__  \\\\_  __ \\   __\\__  \\  ");
            Thread.Sleep(200);
            Console.WriteLine(" /        \\  |_> > __ \\|  | \\/|  |  / __ \\_");
            Thread.Sleep(200);
            Console.WriteLine("/_______  /   __(____  /__|   |__| (____  /");
            Thread.Sleep(200);
            Console.WriteLine("        \\/|__|       \\/                 \\/ ");
            Thread.Sleep(500);
            Console.WriteLine("돌아가려면 아무 키나 누르시오...");
            Console.ReadKey();
        }
    }
}
