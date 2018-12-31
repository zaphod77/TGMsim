﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGMsim
{
    class GameRules
    {
        public Games gameRules;
        public int nextNum = 1;
        public bool hold = false;
        public int hardDrop = 0; //no, sonic, firm

        public int genAttps = 4;

        public int rotation = 0; //TGM, TGM3, SEGA, SRS
        public int generator = 0; //dummy, TGM1, TGM2, TGM3, ACE, 
        public enum Rots { ARS1 = 0, ARS3, SEGA};
        public enum Gens { dummy = 0, TGM1, TGM2, TGM3, ACE, SEGA, EZ};
        public enum Games { SEGA = 0, TGM1, TGM2, TAP, TGM3, ACE, GMX, EXTRA, GUIDELINE}

        //public int baseARE = 30;
        //public int baseARELine = 30;
        //public int baseDAS = 14;
        //public int baseLock = 30;
        //public int baseLineClear = 41;
        public int gravType = 0; //b256, b65536, frames
        public int baseGrav = 4;

        public int lockType = 0; //step-reset, move-reset

        public int creditsLength = 2968;//3238??

        public int fieldW = 10;
        public int fieldH = 22;

        public double FPS = 60.00;
        public int lag = 0;

        //public bool showGrade = true;
        //public int initialGrade = 0;
        //public int variant = 0;

        public string GameName = "error";
        //public string ModeName = "error";

        public Mode mod;


        public List<List<double>> comboTable = new List<List<double>>();

        public List<int> decayRate = new List<int>() { 125, 80, 80, 50, 45, 45, 45, 40, 40, 40, 40, 40, 30, 30, 30, 20, 20, 20, 20, 20, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 10, 10 };
        public List<List<int>> baseGradePts = new List<List<int>>();

        public List<int> gravTable = new List<int>();//TODO: grav table set plz
        public List<int> gravLevels = new List<int>() { 0, 30, 35, 40, 50, 60, 70, 80, 90, 100, 120, 140, 160, 170, 200, 220, 230, 233, 236, 239, 243, 247, 251, 300, 330, 360, 400, 420, 450, 500 };
        public List<List<int>> delayTable = new List<List<int>>();

        //public List<int> gradePointsTGM1 = new List<int> { 0, 400, 800, 1400, 2000, 3500, 5500, 8000, 12000, 16000, 22000, 30000, 40000, 52000, 66000, 82000, 100000, 120000 };

        public List<string> grades = new List<string> { "9", "8", "7", "6", "5", "4", "3", "2", "1", "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8", "S9", "M1", "M2", "M3", "M4", "M5", "M6", "M7", "M8", "M9", "M", "MK", "MV", "MO", "MM", "GM" };

        public List<int> gradeIntTGM2 = new List<int> { 0, 1, 2, 3, 4, 5, 5, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 10, 11, 12, 12, 12, 13, 13, 14, 14, 15, 15, 16, 16, 17 };

        //public List<int> secCools = new List<int> { 52000, 52000, 49000, 45000, 45000, 42000, 42000, 38000, 38000 };
        //public List<int> secRegrets = new List<int> { 90000, 75000, 75000, 68000, 60000, 60000, 50000, 50000, 50000, 50000 };

        /*public struct Gimmick
        {
            public int type; //fading, vanishing, copygarbage, bones, ice, big, random garbage, preset garbage
            public int startLvl;
            public int endLvl;
            public int parameter;
        }*/

        public int id = 0;
        //public int endLevel = 999;
        //public List<int> sections = new List<int>();
        //public bool bigmode = false;
        //public bool g20 = false;
        //public bool shiraseGrades = false;
        public int exam = -1;
        //public int lvlBonus = 0;
        //public int gradedBy = 0; //points, grade points, level, bravo, time
        //public int limitType = 0; //none, line, level, time, bravo
        //public int limit = 0;
        public bool mute = false;
        //public List<Gimmick> gimList = new List<Gimmick>();
        public int bigMove = 1;

        //public Color border = Color.LightGray;

        public GameRules()
        {
            comboTable.Add(new List<double>() { 1.0, 1.2, 1.2, 1.4, 1.4, 1.4, 1.4, 1.5, 1.5, 2.0 });
            comboTable.Add(new List<double>() { 1.0, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2.0, 2.1, 2.5 });
            comboTable.Add(new List<double>() { 1.0, 1.5, 1.8, 2.0, 2.2, 2.3, 2.4, 2.5, 2.6, 3.0 });
            comboTable.Add(new List<double>() { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 });
            baseGradePts.Add(new List<int>() { 10, 10, 10, 10, 10, 5, 5, 5, 5, 5 });
            baseGradePts.Add(new List<int>() { 20, 20, 20, 15, 15, 15, 10, 10, 10, 10 });
            baseGradePts.Add(new List<int>() { 40, 30, 30, 30, 20, 20, 20, 15, 15, 15 });
            baseGradePts.Add(new List<int>() { 50, 40, 40, 40, 40, 30, 30, 30, 30, 30 });
            for (int i = 0; i < 22; i++)
            {
                baseGradePts[0].Add(2);
                baseGradePts[1].Add(12);
                baseGradePts[2].Add(13);
                baseGradePts[3].Add(30);
            }
        }

        public void setup(Games game, int mode, int vari)
        {
            gameRules = game;
            //variant = vari;
            switch (game)
            {
                case Games.SEGA: //SEGA
                    GameName = "SEGA";
                    FPS = 60.00;
                    nextNum = 1;
                    hold = false;
                    hardDrop = 0;
                    rotation = (int)Rots.SEGA;
                    generator = (int)Gens.SEGA;
                    lockType = 0;
                    bigMove = 1;
                    //baseARE = 25;
                    //baseARELine = 25;
                    //baseDAS = 14;
                    //baseLock = 30;
                    //baseLineClear = 40;
                    gravType = 2;
                    baseGrav = 256;
                    genAttps = 6;
                    fieldW = 10;
                    fieldH = 20;
                    //showGrade = false;
                    //gravTable = new List<int> { 5.3, 10.6, 14.2, 17.06, 21.3, 25.6, 32, 42.6, 64, 128, 25.6, 32, 42.6, 64, 128, 256 }; //actual. all final decimal places repeat.
                    gravTable = new List<int> { 48, 24, 18, 15, 12, 10, 8, 6, 4, 2, 10,  8, 6, 4, 2, 1 };
                    gravLevels = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
                    break;
                case Games.TGM1: //TGM
                    GameName = "TGM";
                    FPS = 59.84;
                    nextNum = 1;
                    hold = false;
                    hardDrop = 0;
                    rotation = (int)Rots.ARS1;
                    generator = (int)Gens.TGM1;
                    lag = 2;
                    lockType = 0;
                    bigMove = 1;
                    //baseARE = 30;
                    //baseDAS = 14;
                    //baseLock = 30;
                    //baseLineClear = 41;
                    gravType = 0;
                    baseGrav = 4;
                    genAttps = 4;
                    fieldW = 10;
                    fieldH = 22;
                    creditsLength = 2968; //taken from nullpomino, though estimates place it around 2961. still need to verify
                    //showGrade = true;
                    gravTable = new List<int> { 4, 6, 8, 10, 12, 16, 32, 48, 64, 80, 96, 112, 128, 144, 4, 32, 64, 96, 128, 160, 192, 224, 256, 512, 768, 1024, 1280, 1024, 768, 5120 };
                    break;
                case Games.TGM2: //TGM2
                    GameName = "TGM2";
                    FPS = 61.68;
                    nextNum = 1;
                    hold = false;
                    hardDrop = 1;
                    rotation = (int)Rots.ARS1;
                    generator = (int)Gens.TGM2;
                    lag = 0;
                    lockType = 0;
                    bigMove = 2;
                    gravType = 0;
                    baseGrav = 4;
                    genAttps = 6;
                    fieldW = 10;
                    fieldH = 22;
                    creditsLength = 3238;
                    gravTable = new List<int> { 4, 6, 8, 10, 12, 16, 32, 48, 64, 80, 96, 112, 128, 144, 4, 32, 64, 96, 128, 160, 192, 224, 256, 512, 768, 1024, 1280, 1024, 768, 5120 };
                    break;
                case Games.TAP: //TAP
                    GameName = "TAP";
                    FPS = 61.68;
                    nextNum = 1;
                    hold = false;
                    hardDrop = 1;
                    rotation = (int)Rots.ARS1;
                    generator = (int)Gens.TGM2;
                    lag = 0;
                    lockType = 0;
                    bigMove = 1;
                    gravType = 0;
                    baseGrav = 4;
                    genAttps = 6;
                    fieldW = 10;
                    fieldH = 22;
                    creditsLength = 3238;
                    gravTable = new List<int> { 4, 6, 8, 10, 12, 16, 32, 48, 64, 80, 96, 112, 128, 144, 4, 32, 64, 96, 128, 160, 192, 224, 256, 512, 768, 1024, 1280, 1024, 768, 5120 };
                    break;
                case Games.TGM3: //TI and ACE
                    GameName = "TGM3";
                    FPS = 60.00;
                    nextNum = 3;
                    hold = true;
                    hardDrop = 1;
                    rotation = (int)Rots.ARS3;
                    generator = (int)Gens.TGM3;
                    lag = 3;
                    lockType = 0;
                    bigMove = 2;
                    //baseARE = 25;
                    //baseARELine = 25;
                    //baseDAS = 14;
                    //baseLock = 30;
                    //baseLineClear = 40;
                    gravType = 1;
                    baseGrav = 1024;
                    genAttps = 6;
                    fieldW = 10;
                    fieldH = 22;
                    //showGrade = false;
                    gravTable = new List<int> { 1024, 1536, 2048, 2560, 3072, 4096, 8192, 12288, 16384, 20480, 24576, 28672, 32768, 36864, 1024, 8192, 16348, 24576, 32768, 40960, 49152, 57344, 65536, 131072, 196608, 262144, 327680, 262144, 196608, 1310720 };
                    break;
                case Games.ACE:
                case Games.EXTRA:
                    GameName = "BONUS";
                    FPS = 60.00;
                    nextNum = 3;
                    hold = true;
                    hardDrop = 1;
                    rotation = (int)Rots.ARS3;
                    generator = (int)Gens.TGM3;
                    lag = 3;
                    lockType = 0;
                    bigMove = 2;
                    //baseARE = 25;
                    //baseARELine = 25;
                    //baseDAS = 14;
                    //baseLock = 30;
                    //baseLineClear = 40;
                    gravType = 1;
                    baseGrav = 1024;
                    genAttps = 6;
                    fieldW = 10;
                    fieldH = 22;
                    //showGrade = false;
                    gravTable = new List<int> { 1024, 1536, 2048, 2560, 3072, 4096, 8192, 12288, 16384, 20480, 24576, 28672, 32768, 36864, 1024, 8192, 16348, 24576, 32768, 40960, 49152, 57344, 65536, 131072, 196608, 262144, 327680, 262144, 196608, 1310720 };
                    break;
                case Games.GMX: //GMX?
                    GameName = "XTREME";
                    FPS = 60.00;
                    nextNum = 1;
                    hold = false;
                    hardDrop = 1;
                    rotation = (int)Rots.ARS3;
                    generator = (int)Gens.TGM3;
                    lockType = 0;
                    bigMove = 2;
                    //baseARE = 25;
                    //baseARELine = 25;
                    //baseDAS = 14;
                    //baseLock = 30;
                    //baseLineClear = 40;
                    gravType = 0;
                    baseGrav = 256;
                    genAttps = 6;
                    fieldW = 10;
                    fieldH = 22;
                    //showGrade = false;
                    gravLevels = new List<int>() { 0, 25, 50, 75, 100, 125, 150, 175, 200, 225, 250, 275, 300, 325, 350, 375, 400, 425, 450, 475, 500, 600, 700,  800,  900, 1000};
                    gravTable = new List<int>    { 4, 11, 19, 26,  34,  41,  49,  56,  64,  72,  80,  88,  96, 104, 112, 120, 128, 160, 192, 224, 256, 512, 768, 1024, 1280, 5120};
                    break;
                case Games.GUIDELINE://step reset
                    GameName = "BONUS?";
                    FPS = 60.0;
                    nextNum = 4;
                    hold = true;
                    hardDrop = 1;
                    rotation = (int)Rots.ARS3;
                    generator = (int)Gens.TGM3;
                    lockType = 1;
                    bigMove = 2;
                    gravType = 1;
                    baseGrav = 1024;
                    genAttps = 6;
                    fieldW = 10;
                    fieldH = 24;
                    //showGrade = false;
                    gravTable = new List<int> { 1024, 1536, 2048, 2560, 3072, 4096, 8192, 12288, 16384, 20480, 24576, 28672, 32768, 36864, 1024, 8192, 16348, 24576, 32768, 40960, 49152, 57344, 65536, 131072, 196608, 262144, 327680, 262144, 196608, 1310720 };
                    break;
            }

            id = mode;
            switch (mode)//Master, Death, shirase, sprint, garbage clear, rounds, konoha, grav training, dynamo, endura
            {
                case 0: //Master
                    switch (game)
                    {
                        case Games.TGM1:
                            mod = new M_Master1();
                            break;
                        case Games.TGM2://tgm2
                            mod = new M_Master2();
                            break;
                        case Games.TAP://tap
                            mod = new M_Master2Plus();
                            break;
                        case Games.TGM3://tgm3
                            mod = new M_Master3();
                            break;
                    }
                    break;
                case 1://death
                    mod = new M_Death();
                    break;
                case 2://shirase
                    mod = new M_Shirase();
                    break;
                case 3://sprint
                    /*gradedBy = 4;
                    limitType = 1;
                    border = Color.DarkGreen;*/
                    break;
                case 4://garbage clear
                    /*gradedBy = 4;
                    limitType = 4;
                    limit = 1;
                    sections.Add(999);
                    delayTable.Add(new List<int> { 30 });
                    delayTable.Add(new List<int> { 30 });
                    delayTable.Add(new List<int> { 16 });
                    delayTable.Add(new List<int> { 30 });
                    delayTable.Add(new List<int> { 41 });*/
                    break;
                case 5://rounds
                    mod = new M_IcyShirase();
                    break;
                case 6://konoha
                    generator = (int)Gens.EZ;
                    mod = new M_BigBravoMania();
                    break;
                case 7://20g training
                    mod = new M_Training();
                    break;
                case 8: //segatet
                    mod = new M_SegaTet();
                    break;
                case 9: //miner
                    /*ModeName = "MINER";
                    sections.Add(100);
                    sections.Add(200);
                    sections.Add(300);
                    sections.Add(400);
                    endLevel = 500;
                    delayTable.Add(new List<int> { 27 });
                    delayTable.Add(new List<int> { 27 });
                    delayTable.Add(new List<int> { 14 });
                    delayTable.Add(new List<int> { 30 });
                    delayTable.Add(new List<int> { 40 });*/
                    break;
                case 10: //dynamo
                    mod = new M_Dynamo(vari);
                    break;
                case 11: //endura
                    break;
                case 12: //bloxeed
                    mod = new M_SegaBlox();
                    gravTable = new List<int> { 16, 14, 12, 10, 8, 6, 4, 3, 2, 1, 10, 8, 6, 4, 2, 1 };
                    break;
                case 13: //tgm+
                    mod = new M_Plus();
                    break;
            }
        }
    }
}
