﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.IO;
using System.Drawing.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TGMsim
{
    public partial class Form1 : Form
    {

        GameTimer timer = new GameTimer();
        double FPS = 60.00; //59.84 for TGM1, 61.68 for TGM2, 60.00 for TGM3
        long startTime;
        long interval;

        Controller pad1 = new Controller();
        GameRules rules = new GameRules();

        Image imgBuffer;
        Graphics graphics, drawBuffer;

        Profile player;
        Preferences prefs;

        PrivateFontCollection fonts = new PrivateFontCollection();
        Font f_Maestro;

        int menuState = 0; //title, login, game select, mode select, ingame, results, hiscore roll, custom game, settings

        Field field1;
        Login login;
        GameSelect gSel;
        ModeSelect mSel;
        CheatMenu cMen;

        List<List<GameResult>> hiscoreTable = new List<List<GameResult>>();
        bool saved;

        NAudio.Vorbis.VorbisWaveReader musicStream;
        NAudio.Wave.WaveOutEvent songPlayer = new NAudio.Wave.WaveOutEvent();

        int buffS;
        System.Windows.Media.MediaPlayer s_Start = new System.Windows.Media.MediaPlayer();
        System.Windows.Media.MediaPlayer s_Login = new System.Windows.Media.MediaPlayer();
        System.Windows.Media.MediaPlayer s_GSel = new System.Windows.Media.MediaPlayer();

        public Form1()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.ClientSize = new Size(800, 600);//Size(1280, 780);

            interval = (long)TimeSpan.FromSeconds(1.0 / FPS).TotalMilliseconds;
            fonts.AddFontFile(@"Res\Maestro.ttf");
            FontFamily fontFam = fonts.Families[0];
            f_Maestro = new System.Drawing.Font(fontFam, 16, GraphicsUnit.Pixel);

            imgBuffer = (Image)new Bitmap(this.ClientSize.Width, this.ClientSize.Height);

            hiscoreTable.Add(new List<GameResult>());
            hiscoreTable.Add(new List<GameResult>());
            hiscoreTable.Add(new List<GameResult>());
            hiscoreTable.Add(new List<GameResult>());

            cMen = new CheatMenu();
            player = new Profile();
            prefs = new Preferences(player, pad1);

            readPrefs();

            graphics = this.CreateGraphics();
            drawBuffer = Graphics.FromImage(imgBuffer);


            Audio.addSound(s_Start, "/Res/Audio/SE/SEI_class.wav");
            Audio.addSound(s_Login, "/Res/Audio/SE/SEI_data_ok.wav");
            Audio.addSound(s_GSel, "/Res/Audio/SE/SEI_mode_ok.wav");


            Audio.playMusic("Hello Again");
            
        }

        public void gameLoop()
        {
            timer.start();

            while (this.Created)
            {
                startTime = timer.elapsedTime;

                gameLogic();
                gameRender();

                Application.DoEvents();
                while (timer.elapsedTime - startTime < interval) ;
            }
        }

        void changeMenu(int newMenu)
        {

            switch (newMenu) //activate the new menu
            {  //title, login, game select, mode select, ingame, results, hiscore roll, custom game, settings, cheats
                case 0:
                    menuState = 0;
                    FPS = 60.00;
                    break;
                case 1:
                    menuState = 1;
                    login = new Login();
                    break;
                case 2:
                    if (menuState > 3 && menuState != 8)
                    {
                        Audio.stopMusic();
                        Audio.playMusic("Hello Again");
                    }
                    menuState = 2;
                    FPS = 60.00;
                    pad1.setLag(0);
                    if (gSel == null)
                        gSel = new GameSelect();
                    break;
                case 3:
                    Audio.playSound(s_GSel);
                    menuState = 3;
                    if (gSel.menuSelection != 4)
                    loadHiscores(gSel.menuSelection + 1);
                    mSel = new ModeSelect(gSel.menuSelection);
                    break;
                case 4:
                    saved = false;
                    menuState = 4;
                    switch (gSel.menuSelection)
                    {
                        case 0:
                            FPS = 59.84;
                            if (prefs.delay == true) pad1.setLag(2);
                            break;
                        case 1:
                        case 2:
                            FPS = 61.68;
                            break;
                        case 3:
                        case 4:
                        case 5:
                            FPS = 60.00;
                            if (prefs.delay == true) pad1.setLag(4);
                            break;
                    }
                    setupGame();
                    break;

                case 6: //hiscores
                    menuState = 6;
                    //loadHiscores(mSel.game + 1);
                    //Audio.stopMusic();
                    //Audio.playMusic("Hiscores");
                    break;

                case 8:
                    menuState = 8;
                    readPrefs();
                    break;
                case 9:
                    menuState = 9;
                    break;
            }

            menuState = newMenu;
        }

        void gameLogic()
        {
            pad1.poll();

            //deal with game logic
            switch (menuState)
            {
                case 0: //title
                    titleLogic();
                    break;
                case 1: //login
                    loginLogic();
                    break;
                case 2: //game select
                    gSel.logic(pad1);
                    if ((pad1.inputRot1 | pad1.inputRot3) == 1)
                    {
                        if (gSel.prompt)
                        {
                            if (gSel.pSel == 1)
                            {
                                cMen = new CheatMenu();
                                player.name = "   ";
                                gSel.prompt = false;
                                changeMenu(1);
                            }
                            else
                            {
                                gSel.prompt = false;
                            }
                        }
                        else
                        {
                            if (gSel.menuSelection == 7) //settings
                                changeMenu(8);
                            else if (gSel.menuSelection == 6) //Bonus
                            {
                                changeMenu(3);
                            }
                            else
                            {
                                //rules.setGame(gSel.menuSelection + 1);
                                changeMenu(3);
                            }
                        }
                    }
                    else
                    {
                        if (pad1.inputRot2 == 1)
                        {
                            if (gSel.prompt)
                                gSel.prompt = false;
                            else
                            {
                                gSel.pSel = 0;
                                gSel.prompt = true;
                            }
                        }
                    }
                    break;
                case 3: //mode select
                    mSel.logic(pad1);
                    if ((pad1.inputRot1 | pad1.inputRot3) == 1)
                    {
                        rules = new GameRules();
                        //TODO: change rules based on what mode is selected
                        if (gSel.menuSelection < 5)
                            changeMenu(4);
                        else if (gSel.menuSelection == 5 && mSel.selection != 0) //konoha
                        {
                            changeMenu(4);
                        }
                        else
                        {
                            switch (mSel.selection)
                            {
                                case 0://Custom
                                    changeMenu(7);
                                    break;
                                case 1://eternal shirase
                                    rules.setup(4,2);
                                    rules.endLevel = 0;
                                    //todo: add more gimmicks, loop?
                                    break;
                                case 2://garbage
                                    rules.setup(4,4);
                                    saved = false;
                                    menuState = 4;
                                    Audio.stopMusic();
                                    field1 = new Field(pad1, rules, musicStream);
                                    break;
                                case 3://20g practice
                                    changeMenu(4);
                                    break;
                            }
                        }
                    }
                    if (pad1.inputRot2 == 1)
                        changeMenu(2);
                    if (pad1.inputHold == 1 && mSel.game != 4)
                        changeMenu(6);//hiscores for mode
                    break;
                case 4: //ingame
                    field1.logic();
                    if (field1.gameRunning == false)
                    {
                        //test and save a hiscore ONCE
                        if (saved == false)
                        {
                            field1.results.username = player.name;
                            if (rules.gameRules == 4 && field1.ruleset.id == 0 && player.name != "   ")
                            {
                                //add result to history
                                for (int i = 0; i < 6; i++)
                                {
                                    player.TIHistory[i] = player.TIHistory[i + 1];
                                }
                                player.TIHistory[6] = field1.results.grade;

                                //if exam, check results for promotion
                                if (field1.ruleset.exam != -1)
                                {
                                    if (field1.results.grade >= field1.ruleset.exam)
                                    {
                                        player.TIGrade = field1.ruleset.exam;
                                    }
                                }

                                //scale GM back if unqualified
                                else if (field1.results.grade == 32 && player.TIGrade != 32)
                                {
                                    field1.results.grade = 31;
                                }
                            }

                            if (!field1.cheating && field1.ruleset.exam == -1) //TODO: check if this is a replay as well
                                field1.newHiscore = testHiscore(field1.results);
                            if (player.name != "   ")
                                player.updateUser();
                            saved = true;
                        }

                    }
                    if (field1.cont == true)
                    {
                        setupGame();
                    }
                    if (field1.exit == true)
                        changeMenu(2);
                    break;
                case 6://hiscores
                    if (pad1.inputPressedRot2)
                        changeMenu(3);
                    break;
                case 7://custom game
                    if (pad1.inputPressedRot2)
                        changeMenu(2);
                    break;
                case 8://settings
                    if (pad1.inputRot2 == 1 && prefs.menuState == 0)
                    {
                        pad1 = prefs.nPad;
                        savePrefs();
                        changeMenu(2);
                    }
                    prefs.logic();
                    break;
                case 9://cheats
                    if (pad1.inputStart == 1)
                        changeMenu(2);
                    cMen.logic(pad1);
                    break;
            }
        }

        private void loginLogic()
        {
            if (player.name == "   ")
            {

                login.logic(pad1);

                if (login.loggedin)
                {
                    pad1.inputStart = 0;
                    player = login.temp;
                    Audio.playSound(s_Login);
                    savePrefs();
                    if (player.name == "   ")
                        changeMenu(9);
                    else
                        changeMenu(2);
                }
            } else {
                player.readUserData();
                changeMenu(2);
            }
        }

        private void titleLogic()
        {

            if (pad1.inputStart == 1)
            {
                pad1.inputStart = 0;
                Audio.playSound(s_Start);
                changeMenu(1);
            }
        }

        void gameRender()
        {
            //draw temp BG so bleeding doesn't occur
            drawBuffer.FillRectangle(new SolidBrush(Color.Black), this.ClientRectangle);


            switch (menuState)
            {
                case 0:
                    drawBuffer.DrawString("TGM sim title screen thingy", DefaultFont, new SolidBrush(Color.White), 325, 250);
                    drawBuffer.DrawString("PRESS START", f_Maestro, new SolidBrush(Color.White), 350, 300);
                    break;
                case 1:
                    drawBuffer.DrawString("login", DefaultFont, new SolidBrush(Color.White), 5, 5);
                    login.render(drawBuffer);
                    break;
                case 2:
                    drawBuffer.DrawString("game select", DefaultFont, new SolidBrush(Color.White), 5, 5);
                    gSel.render(drawBuffer);
                    break;
                case 3:
                    drawBuffer.DrawString("mode select", DefaultFont, new SolidBrush(Color.White), 5, 5);
                    mSel.render(drawBuffer);
                    break;
                case 4:
                    field1.draw(drawBuffer);
                    break;
                case 6:
                    drawBuffer.DrawString("hiscores", DefaultFont, new SolidBrush(Color.White), 5, 5);

                    for (int i = 0; i < 6; i++ )
                    {
                        drawBuffer.DrawString(hiscoreTable[mSel.game][i].username, DefaultFont, new SolidBrush(Color.White), 250, 100 + 30 * i);
                        if (mSel.game == 3)
                            drawBuffer.DrawString(hiscoreTable[mSel.game][i].level.ToString(), DefaultFont, new SolidBrush(Color.White), 290, 100 + 30 * i);
                        drawBuffer.DrawString(rules.grades[hiscoreTable[mSel.game][i].grade], DefaultFont, new SolidBrush(Color.White), 330, 100 + 30 * i);
                        var temptimeVAR = hiscoreTable[mSel.game][i].time;
                        var min = (int)Math.Floor((double)temptimeVAR / 60000);
                        temptimeVAR -= min * 60000;
                        var sec = (int)Math.Floor((double)temptimeVAR / 1000);
                        temptimeVAR -= sec * 1000;
                        var msec = (int)temptimeVAR;
                        var msec10 = (int)(msec / 10);
                        drawBuffer.DrawString(string.Format("{0,2:00}:{1,2:00}:{2,2:00}", min, sec, msec10), DefaultFont, new SolidBrush(Color.White), 360, 100 + 30 * i);
                        if (mSel.game != 0)
                        {
                            for (int j = 0; j < 6; j++)
                            {
                                drawBuffer.DrawString(hiscoreTable[mSel.game][i].medals[j].ToString(), DefaultFont, new SolidBrush(Color.White), 420 + 10*j, 100 + 30 * i);
                            }
                        }
                        if (hiscoreTable[mSel.game][i].lineC == 1)
                        {
                            drawBuffer.DrawLine(new Pen(Color.LimeGreen), 240, 100 + 30 * i, 550, 100 + 30 * i);
                        }
                        if (hiscoreTable[mSel.game][i].lineC == 2)
                        {
                            drawBuffer.DrawLine(new Pen(Color.Orange), 240, 100 + 30 * i, 550, 100 + 30 * i);
                        }
                    }

                        break;
                case 8:
                    drawBuffer.DrawString("preferences", DefaultFont, new SolidBrush(Color.White), 5, 5);
                    prefs.render(drawBuffer);
                    break;
                case 9:
                    drawBuffer.DrawString("cheats", DefaultFont, new SolidBrush(Color.White), 5, 5);
                    cMen.render(drawBuffer);
                    break;
            }
            if (menuState > 1)
                drawBuffer.DrawString(player.name, f_Maestro, new SolidBrush(Color.White), 765, 5);

#if DEBUG
            SolidBrush debugBrush = new SolidBrush(Color.White);
            //denote debug
            drawBuffer.DrawString("DEBUG", DefaultFont, debugBrush, 20, 710);
            //draw the current inputs
            drawBuffer.DrawString(Keyboard.IsKeyDown(pad1.keyUp) ? "1" : "0", DefaultFont, debugBrush, 28, 720);
            drawBuffer.DrawString(Keyboard.IsKeyDown(pad1.keyLeft) ? "1" : "0", DefaultFont, debugBrush, 20, 730);
            drawBuffer.DrawString(Keyboard.IsKeyDown(pad1.keyDown) ? "1" : "0", DefaultFont, debugBrush, 28, 740);
            drawBuffer.DrawString(Keyboard.IsKeyDown(pad1.keyRight) ? "1" : "0", DefaultFont, debugBrush, 36, 730);
            drawBuffer.DrawString(Keyboard.IsKeyDown(pad1.keyRot1) ? "1" : "0", DefaultFont, debugBrush, 50, 720);
            drawBuffer.DrawString(Keyboard.IsKeyDown(pad1.keyRot2) ? "1" : "0", DefaultFont, debugBrush, 58, 720);
            drawBuffer.DrawString(Keyboard.IsKeyDown(pad1.keyRot3) ? "1" : "0", DefaultFont, debugBrush, 66, 720);
            drawBuffer.DrawString(Keyboard.IsKeyDown(pad1.keyHold) ? "1" : "0", DefaultFont, debugBrush, 50, 730);
            drawBuffer.DrawString(Keyboard.IsKeyDown(pad1.keyStart) ? "1" : "0", DefaultFont, debugBrush, 74, 730);

#endif

            //draw the buffer, then set to refresh
            this.BackgroundImage = imgBuffer;
            this.Invalidate();

        }

        private int checkExam()
        {
            List<int> cream = new List<int>(player.TIHistory);

            //cream = player.TIHistory;
            cream.Sort();

            int avg = (cream[5] + cream[4] + cream[3])/3;

            Random rand = new Random();
            if (avg > player.TIGrade && rand.Next(2) == 0)
                return avg;
            return -1;
            
        }

        private void setupGame()
        {
            //Mode m = new Mode();
            Audio.stopMusic();
            if (mSel.game == 3 && mSel.selection == 1)//shirase
                rules.setup(4,2);
            else if (mSel.game == 5 && mSel.selection == 1)//rounds
                rules.setup(6,5);
            else if (mSel.game == 5 && mSel.selection == 2)//konoha
                rules.setup(6,6);
            else if (mSel.game == 6 && mSel.selection == 3)//20G
                rules.setup(7,7);
            else
                rules.setup(mSel.game + 1, mSel.selection);

            //m.mute = prefs.muted;

            if (player.name == "   ")
            {
                if (rules.id != 6)
                    rules.bigmode = cMen.cheats[3];
                rules.mute = cMen.cheats[4];
            }

            if (mSel.game == 3 && rules.id == 0 && player.name != "   ")
                rules.exam = checkExam();

            field1 = new Field(pad1, rules, musicStream);
            if (player.name == "   ")
            {
                field1.godmode = cMen.cheats[0];
                if (cMen.cheats[1])
                    field1.g20 = cMen.cheats[1];
                field1.g0 = cMen.cheats[2];
                if (field1.godmode || field1.g0)
                    field1.cheating = true;
                if (cMen.cheats[5])
                    field1.w4 = true;
            }
            saved = false;
        }

        private bool testHiscore(GameResult gameResult)
        {
            if (hiscoreTable.Count == 0) //oh no! error reading the file!
                return false;

            switch (gameResult.game)
            {
                case 0:
                    for (int i = 0; i < hiscoreTable[0].Count; i++ ) //for each entry in TGM1
                    {
                        if (hiscoreTable[0][i].grade < gameResult.grade)
                        {
                            saveHiscore(gameResult, gameResult.game, i);
                            return true;
                        }
                        if (hiscoreTable[0][i].grade == gameResult.grade)
                        {
                            //compare time
                            if (hiscoreTable[0][i].time > gameResult.time)
                            {
                                saveHiscore(gameResult, gameResult.game, i);
                                return true;
                            }
                        }
                        //else try the next one.
                    }
                    break;
                case 4:
                    for (int i = 0; i < hiscoreTable[0].Count; i++ )
                    {
                        if (hiscoreTable[4][i].level < gameResult.level)
                        {
                            saveHiscore(gameResult, gameResult.game, i);
                            return true;
                        }
                        if (hiscoreTable[4][i].level == gameResult.level)
                        {
                            if (hiscoreTable[4][i].grade < gameResult.grade)
                            {
                                saveHiscore(gameResult, gameResult.game, i);
                                return true;
                            }
                            if (hiscoreTable[4][i].grade == gameResult.grade)
                            {
                                if (hiscoreTable[gameResult.game][i].time > gameResult.time)
                                {
                                    saveHiscore(gameResult, gameResult.game, i);
                                    return true;
                                }
                            }
                        }
                        
                    }
                    break;
                case 5:
                case 6:
                    return false;
                    //break;
                default:
                    for (int i = 0; i < hiscoreTable[gameResult.game].Count; i++)
                    {
                        if (hiscoreTable[gameResult.game][i].grade < gameResult.grade)
                        {
                            saveHiscore(gameResult, gameResult.game, i);
                            return true;
                        }
                        if (hiscoreTable[gameResult.game][i].grade == gameResult.grade)
                        {
                            //compare line
                            if (hiscoreTable[gameResult.game][i].lineC < gameResult.lineC)
                            {
                                saveHiscore(gameResult, gameResult.game, i);
                                return true;
                            }
                            //compare time
                            if (hiscoreTable[gameResult.game][i].time > gameResult.time)
                            {
                                saveHiscore(gameResult, gameResult.game, i);
                                return true;
                            }
                        }
                    }
                    break;
        }
            return false;
        }

        private void saveHiscore(GameResult gameResult, int g, int place)
        {

            hiscoreTable[g].Insert(place, gameResult);
            hiscoreTable[g].RemoveAt(hiscoreTable[g].Count - 1);

            string hiFile = "Sav/gm" + (g+1) + ".dat";
            File.Delete(hiFile);
            using (FileStream fsStream = new FileStream(hiFile, FileMode.Create))
            using (BinaryWriter sw = new BinaryWriter(fsStream, Encoding.UTF8))
            {
                for (int i = 0; i < hiscoreTable[g].Count; i++)
                {
                    sw.Write(hiscoreTable[g][i].username);
                    sw.Write(hiscoreTable[g][i].grade);
                    sw.Write(hiscoreTable[g][i].time);
                    if (g != 0)
                    {
                        for (int j = 0; j < 6; j++ )
                            sw.Write((byte)(hiscoreTable[g][i].medals[j] & 0xFF));
                    }
                    if (g == 1 || g == 2)
                        sw.Write((byte)hiscoreTable[g][i].lineC);
                    if (g == 3)
                        sw.Write((int)hiscoreTable[g][i].level);
                }
            }
        }
        
        private void loadHiscores (int game)
        {
            string filename = "Sav/gm" + game.ToString() + ".dat";
            if (!File.Exists(filename))
            {
                if (game == 1)
                    defaultTGMScores();
                if (game == 2)
                    defaultTGM2Scores();
                if (game == 3)
                    defaultTAPScores();
                if (game == 4)
                    defaultTGM3Scores();
                if (game == 5)
                    return;
                if (game == 6)
                    return;
                if (game == 7)
                    return;
            }
            
            BinaryReader scores = new BinaryReader(File.OpenRead(filename));
            //otherwise, load up the hiscores into memory!

            bool reading = true;
            while (true)
            {

                GameResult tempRes = new GameResult();
                switch (game)
                {
                    case 1:
                        tempRes.username = scores.ReadString();
                        tempRes.grade = scores.ReadInt32();
                        tempRes.time = scores.ReadInt64();
                        hiscoreTable[game - 1].Add(tempRes);
                        if (scores.BaseStream.Position == scores.BaseStream.Length)
                            reading = false;
                        break;
                    case 4:
                        tempRes.username = scores.ReadString();
                        tempRes.grade = scores.ReadInt32();
                        tempRes.time = scores.ReadInt64();
                        tempRes.medals = new List<int>();
                        for (int i = 0; i < 6; i++)
                        {
                            tempRes.medals.Add((int)scores.ReadByte());
                        }
                        tempRes.level = scores.ReadInt32();
                        hiscoreTable[game - 1].Add(tempRes);
                        if (scores.BaseStream.Position == scores.BaseStream.Length)
                            reading = false;
                        break;
                    default:
                        tempRes.username = scores.ReadString();
                        tempRes.grade = scores.ReadInt32();
                        tempRes.time = scores.ReadInt64();
                        tempRes.medals = new List<int>();
                        for (int i = 0; i < 6; i++)
                        {
                            tempRes.medals.Add((int)scores.ReadByte());
                        }
                        tempRes.lineC = (int)scores.ReadByte();
                        hiscoreTable[game - 1].Add(tempRes);
                        if (scores.BaseStream.Position == scores.BaseStream.Length)
                            reading = false;
                        break;

                }
                if (reading == false)
                    break;
            }
        }

        public bool defaultTGMScores()
        {
            using (FileStream fsStream = new FileStream("Sav/gm1.dat", FileMode.OpenOrCreate))
            using (BinaryWriter sw = new BinaryWriter(fsStream, Encoding.UTF8))
            {
                long temptime;
                sw.Write("SAK");
                sw.Write(11);
                temptime = 540000;
                sw.Write(temptime);
                sw.Write("CHI");
                sw.Write(10);
                temptime = 480000;
                sw.Write(temptime);
                sw.Write("NAI");
                sw.Write(9);
                temptime = 420000;
                sw.Write(temptime);
                sw.Write("MIZ");
                sw.Write(6);
                temptime = 360000;
                sw.Write(temptime);
                sw.Write("KAR");
                sw.Write(5);
                temptime = 300000;
                sw.Write(temptime);
                sw.Write("NAG");
                sw.Write(4);
                temptime = 240000;
                sw.Write(temptime);
            }
            return true;
        }
        public bool defaultTGM2Scores()
        {
            using (FileStream fsStream = new FileStream("Sav/gm2.dat", FileMode.Create))
            using (BinaryWriter sw = new BinaryWriter(fsStream, Encoding.UTF8))
            {
                long temptime;
                sw.Write("T.A");
                sw.Write(9);
                temptime = 1200000;
                sw.Write(temptime);
                sw.Write(new byte[7]);
                sw.Write("T.A");
                sw.Write(6);
                temptime = 1080000;
                sw.Write(temptime);
                sw.Write(new byte[7]);
                sw.Write("T.A");
                sw.Write(3);
                temptime = 960000;
                sw.Write(temptime);
                sw.Write(new byte[7]);
                sw.Write("T.A");
                sw.Write(3);
                temptime = 1200000;
                sw.Write(temptime);
                sw.Write(new byte[7]);
                sw.Write("T.A");
                sw.Write(2);
                temptime = 1080000;
                sw.Write(temptime);
                sw.Write(new byte[7]);
                sw.Write("T.A");
                sw.Write(1);
                temptime = 960000;
                sw.Write(temptime);
                sw.Write(new byte[7]);
            }
            return true;
        }

        public bool defaultTAPScores()
        {
            using (FileStream fsStream = new FileStream("Sav/gm3.dat", FileMode.Create))
            using (BinaryWriter sw = new BinaryWriter(fsStream, Encoding.UTF8))
            {
                long temptime;
                sw.Write("T.A");
                sw.Write(9);
                temptime = 1200000;
                sw.Write(temptime);
                sw.Write(new byte[7]);
                sw.Write("T.A");
                sw.Write(6);
                temptime = 1080000;
                sw.Write(temptime);
                sw.Write(new byte[7]);
                sw.Write("T.A");
                sw.Write(3);
                temptime = 960000;
                sw.Write(temptime);
                sw.Write(new byte[7]);
                sw.Write("T.A");
                sw.Write(3);
                temptime = 1200000;
                sw.Write(temptime);
                sw.Write(new byte[7]);
                sw.Write("T.A");
                sw.Write(2);
                temptime = 1080000;
                sw.Write(temptime);
                sw.Write(new byte[7]);
                sw.Write("T.A");
                sw.Write(1);
                temptime = 960000;
                sw.Write(temptime);
                sw.Write(new byte[7]);
            }
            return true;
        }
        public bool defaultTGM3Scores()
        {
            using (FileStream fsStream = new FileStream("Sav/gm4.dat", FileMode.Create))
            using (BinaryWriter sw = new BinaryWriter(fsStream, Encoding.UTF8))
            {
                long temptime;
                sw.Write("ARK");
                sw.Write(6);
                temptime = 1200000;
                sw.Write(temptime);
                sw.Write(new byte[6]);
                sw.Write(500);
                sw.Write("ARK");
                sw.Write(4);
                temptime = 1080000;
                sw.Write(temptime);
                sw.Write(new byte[6]);
                sw.Write(400);
                sw.Write("ARK");
                sw.Write(3);
                temptime = 1200000;
                sw.Write(temptime);
                sw.Write(new byte[6]);
                sw.Write(400);
                sw.Write("ARK");
                sw.Write(2);
                temptime = 960000;
                sw.Write(temptime);
                sw.Write(new byte[6]);
                sw.Write(300);
                sw.Write("ARK");
                sw.Write(2);
                temptime = 1080000;
                sw.Write(temptime);
                sw.Write(new byte[6]);
                sw.Write(300);
                sw.Write("ARK");
                sw.Write(1);
                temptime = 960000;
                sw.Write(temptime);
                sw.Write(new byte[6]);
                sw.Write(200);
            }
            return true;
        }

        private void savePrefs()
        {
            using (FileStream fsStream = new FileStream("Sav/prefs.dat", FileMode.Create))
            using (BinaryWriter sw = new BinaryWriter(fsStream, Encoding.UTF8))
            {
                sw.Write(prefs.delay);
                sw.Write((char)prefs.nPad.keyUp);
                sw.Write((char)prefs.nPad.keyDown);
                sw.Write((char)prefs.nPad.keyLeft);
                sw.Write((char)prefs.nPad.keyRight);
                sw.Write((char)prefs.nPad.keyRot1);
                sw.Write((char)prefs.nPad.keyRot2);
                sw.Write((char)prefs.nPad.keyRot3);
                sw.Write((char)prefs.nPad.keyHold);
                sw.Write((char)prefs.nPad.keyStart);
                int temp = ((int)Math.Floor(Audio.musVol * 10) << 4) + (int)Math.Floor(Audio.sfxVol * 10);
                sw.Write((char)temp);
                sw.Write(player.name);
            }
        }

        private void readPrefs()
        {
            BinaryReader prf = new BinaryReader(File.OpenRead("Sav/prefs.dat"));
            prefs.delay = prf.ReadBoolean();
            prefs.nPad.keyUp = (Key)prf.ReadByte();
            prefs.nPad.keyDown = (Key)prf.ReadByte();
            prefs.nPad.keyLeft = (Key)prf.ReadByte();
            prefs.nPad.keyRight = (Key)prf.ReadByte();
            prefs.nPad.keyRot1 = (Key)prf.ReadByte();
            prefs.nPad.keyRot2 = (Key)prf.ReadByte();
            prefs.nPad.keyRot3 = (Key)prf.ReadByte();
            prefs.nPad.keyHold = (Key)prf.ReadByte();
            prefs.nPad.keyStart = (Key)prf.ReadByte();
            byte temp = prf.ReadByte();
            Audio.musVol = (float)((temp >> 4) & 0x0F)/10;
            Audio.sfxVol = (float)(temp & 0x0F) / 10;
            player.name = prf.ReadString();
            prf.Close();
        }
    }
}
