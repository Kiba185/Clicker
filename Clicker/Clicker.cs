using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.IO;

namespace Clicker
{
    public partial class Clicker : Form
    {
        public Clicker()
        {
            InitializeComponent();
            panel66.Location = new System.Drawing.Point(0, 0);
        }
        //admin režim
        string admin_heslo = "4321";
        bool admin_rezim = false;

        //save / load game
        private string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "data.json");
        private string key = "1234567890123456"; // 16 znaků pro AES-128
        private DataContainer data = new DataContainer { x = 42, y = 84, z = 128, a = 256, b = 512 };



        //Baterka
        int energy = 0;
        int maxEnergy = 100;
        int baseEnergy = 1;
        int energyNasob = 1;
        int vyskaplnostibaterie = 235;
        int sirkaplnostibateriesize = 1;
        int sirkaplnostibaterielocation = 1;

        //Currrency save
        string penize_save = "0";
        string gemy_save = "0";

        //Currency/Suroviny
        int penize = 0;
        int basePenize = 1;
        int nasob = 1;
        int gemy = 0;

        int wood = 0;
        int stone = 0;
        int wheat = 0;
        int plank = 0;

        int woodWait = 0;
        int stoneWait = 0;
        int wheatWait = 0;
        int plankWait = 0;

        //Stats
        int totalClicks = 0;
        int totalEnergy = 0;
        int totalPenize = 0;
        int achievments = 0;
        int machines = 1;
        int resources = 0;
        int sellRecources = 0;

        //Klikání
        int lastClick = 0;
        int maxLastClick = 5;

        //Other
        bool checkClicksOn = false;
        bool penizeGiving = false;

        //Machines
        int autoEnergyUpgrade = 0;
        int forestUpgrade = 0;
        int mineUpgrade = 0;
        int farmUpgrade = 0;
        int utilityBuildingUpgrade = 0;
        
        void update()
        {
            float penizeText = penize;
            tlacitko.Text = "Energy: " + energy + "/" + maxEnergy;
            button6.Text = "Energy: " + energy + "/" + maxEnergy;
            label15.Text = "" + gemy;
            button19.Text = "Větší klikání - " + 10 * (energyNasob*3) + "$";
            button5.Text = "Double peníze - " + 50 * (nasob) + "$";
            button12.Text = "LastClick Check Upgrade: " + 150 * (maxLastClick - 4) + "$";
            autoClick.Text = "Solár: " + 5 + "K $";
            button16.Text = "Forest: " + 30 + "K $";
            button17.Text = "Mine: " + 50 + "K $";
            button18.Text = "Farm: " + 15 + "K $";
            button20.Text = "Sawmill: " + 100 + "K $";
            


            //Peníze ukazatel
            if (penize < 1000) { label2.Text = ""+penize; }
            else if(penize > 1000 && penize < 100000) { label2.Text = Math.Round(penizeText/1000,2) + "K"; }
            else if (penize > 100000 && penize < 1000000) { label2.Text = Math.Round(penizeText / 1000) + "K"; }
            else if (penize > 1000000 && penize < 100000000) { label2.Text = Math.Round(penizeText / 1000000, 2) + "M"; }
            else if (penize > 100000000 && penize < 1000000000) { label2.Text = Math.Round(penizeText / 1000000) + "M"; }
            else { label2.Text = Math.Round(penizeText / 1000000000, 2) + "B"; }


            if (maxEnergy < 500)
            {
                button3.Text = "Energy kapacita upgrade: " + maxEnergy * 2 + "$";
            }
            else button3.Visible = false;
            
            //Ukazatel energie - max 148, 275 
            energy10.Size = new Size(125, 0 + (energy*vyskaplnostibaterie/maxEnergy));
            energy10.Location = new System.Drawing.Point(27, 317 - (energy*vyskaplnostibaterie/maxEnergy));
            energy10.Visible = false;
            if (energy > 0)
            {
                energy10.Visible = true;
            
            }
            else { energy10.Visible = false; }

            
        }

        void updateStorage()
        {
            
            numericUpDown1.Maximum = wood ;
            numericUpDown2.Maximum = stone ;
            numericUpDown3.Maximum = wheat ;
            numericUpDown4.Maximum = plank ;

            numericUpDown1.Value = wood;
            numericUpDown2.Value = stone;
            numericUpDown3.Value = wheat;
            numericUpDown4.Value = plank;

            button25.Text = "Vyzvednout: " + woodWait ;
            button26.Text = "Vyzvednout: " + stoneWait;
            button27.Text = "Vyzvednout: " + wheatWait;
            button28.Text = "Vyzvednout: " + plankWait;
        }

        async Task autoEnergy()
        {
            while (true)
            {
                energy += autoEnergyUpgrade;
                totalEnergy += autoEnergyUpgrade;
                if (energy > maxEnergy) { energy = maxEnergy; }
                update();
                await Task.Delay(1000);
            }

        }

        async Task forest()
        {
            while (true)
            {
                if (energy > 100)
                {
                    woodWait += 1;
                    resources += 1;
                    updateStorage();
                    energy -= 50;
                    await Task.Delay(5000);
                }
                else { await Task.Delay(1000); }
            }
        }

        async Task mine()
        {
            while (true)
            {
                if (energy > 100)
                {
                    stoneWait += 1;
                    resources += 1;
                    updateStorage();
                    energy -= 75;
                    await Task.Delay(5000);
                }
                else { await Task.Delay(1000); }
            }
        }

        async Task farm()
        {
            while (true)
            {
                if (energy > 100)
                {
                    wheatWait += 1;
                    resources += 1;
                    updateStorage();
                    energy -= 30;
                    await Task.Delay(5000);
                }
                else { await Task.Delay(1000); }
            }
        }

        async Task utilityBuilding()
        {
            while (true)
            {
                if (wood >= 2) {
                    if (energy > 250)
                    {
                        wood -= 2;
                        plankWait += 1;
                        resources += 1;
                        updateStorage();
                        energy -= 100;
                        await Task.Delay(5000);
                    }
                    else { await Task.Delay(1000); }
                }
                else { await Task.Delay(1000); }
            }
        }

        //Kontrola posledního kliku
        async Task checkClicks()
        {
            while (true)
            {
                if (lastClick > 0)
                {
                    lastClick -= 1;
                    if (lastClick == 0)
                    {
                        while (lastClick == 0)
                        {
                            if (energy > 0)
                            {
                                energy -= 10;
                            }
                            if (energy < 0)
                            {
                                energy = 0;
                            }
                            update();
                            await Task.Delay(250);
                        }
                    }
                }
                await Task.Delay(1000);
            }
        }
        //Dávání peněz když je energie
        async Task penizeGive()
        {
            while (true)
            {
                if (energy > 0)
                {
                    penize += basePenize * nasob;
                    totalPenize += basePenize * nasob;
                    update();
                    if (energy >= 20)
                    {
                        await Task.Delay(100000 / (energy + 1));
                    }
                    else { await Task.Delay(5000); }
                    //Console.WriteLine((10000 / (energy + 1)).ToString());
                }
                else {await Task.Delay(1000); }
            }
        }

        //Klikání
        private async void button1_Click(object sender, EventArgs e)
        {
            
            lastClick = maxLastClick;
            if (energy < maxEnergy)
            {
                energy += baseEnergy * energyNasob;
                totalEnergy += baseEnergy * energyNasob;
                if (energy > maxEnergy) { energy = maxEnergy; }
                totalClicks += 1;
                update();
            }
            if (checkClicksOn == false)
            {
                checkClicksOn = true;


                await checkClicks();
            }
            if (penizeGiving == false)
            {
                penizeGiving = true;
                await penizeGive();
            }
            button6.Visible = true;
            tlacitko.Visible = false;
        }

        //SHOP

        //Větší klikání
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (penize >= 10 * (energyNasob * 3))
            {
                penize -= (10 * (energyNasob * 3));
                energyNasob += 1;
                update();   

            }
        }
        //Přidávání kapacity energie(+baterka) - přidat pak aby to přidávalo/zvětšovalo další baterky
        private void button3_Click(object sender, EventArgs e)
        {
            if (maxEnergy < 500)
            {
                if (penize >= maxEnergy*2)
                {     
                    penize -= maxEnergy*2;
                    maxEnergy += 50;
                    update();
                }
            }
        }
        //Double peníze - zatím
        private void button5_Click_1(object sender, EventArgs e)
        {
            if (penize >= 50*(nasob))
            {
                penize -= 50*(nasob);
                nasob += 1;
                update();
            }
        }
        //MaxLastClick Upgrade - zatím
        private void button12_Click(object sender, EventArgs e)
        {
            //button5
            if(penize >= 150 * (maxLastClick - 4))
            {
                penize -= 150 * (maxLastClick - 4);
                maxLastClick += 1;
            }
        }
        //AutoEnergy(Solar)
        private async void button2_Click(object sender, EventArgs e)
        {
            if (penize >= 5000 && autoEnergyUpgrade == 0)
            {
                penize -= 5000;
                panel54.Visible = true;
                update();
                autoEnergyUpgrade = 1;
                machines += 1;
                autoClick.Visible = false;
                await autoEnergy();
            }
        }
        //Forest
        private async void button16_Click_1(object sender, EventArgs e)
        {
            if (penize >= 30000 && forestUpgrade == 0)
            {
                penize -= 30000;
                panel55.Visible = true;
                button25.Visible = true;
                update();
                forestUpgrade = 1;
                machines += 1;
                button16.Visible = false;
                await forest();
            }
        }

        //Mine
        private async void button17_Click(object sender, EventArgs e)
        {
            if (penize >= 50000 && mineUpgrade == 0)
            {
                penize -= 50000;
                panel58.Visible = true;
                button26.Visible = true;
                update();
                mineUpgrade = 1;
                machines += 1;
                button17.Visible = false;
                await mine();
            }
        }
        //Farm
        private async void button18_Click(object sender, EventArgs e)
        {
            if (penize >= 15000 && farmUpgrade == 0)
            {
                penize -= 15000;
                panel57.Visible = true;
                button27.Visible = true;
                update();
                farmUpgrade = 1;
                machines += 1;
                button18.Visible = false;
                await farm();
            }
        }
        //UtilityBuilding
        private async void button20_Click(object sender, EventArgs e)
        {
            if (penize >= 100000 && utilityBuildingUpgrade == 0)
            {
                penize -= 100000;
                panel56.Visible = true;
                button28.Visible = true;
                update();
                utilityBuildingUpgrade = 1;
                machines += 1;
                button20.Visible = false;
                await utilityBuilding();
            }
        }


        //Achievmenty
        bool Aclick = false; //16
        bool Abiggerclick = false; //14
        bool Abiggercapacity = false; //18
        bool Astovka = false; //20
        bool Atisicovka = false; //22
        bool Astotisicovka = false; //24
        bool Alastclickupgrade = false; //26
        bool Aautoclick = false; //28
        bool Amaxenergy = false; //30
        bool Atisicclicku = false; //32
        bool Afinale = false; //ende verze 0.06 - 34
        bool Amachines = false; //76
        bool AstoResources = false; //70
        bool AsellResource = false; //79
        bool A100Kprodej = false; //73
        
        void UpdateAchievments() //zk velikost 87,29
        {
          if (totalClicks > 0 && Aclick == false) {
                Aclick = true;
                achievments += 1;
                gemy += 1;
                panel16.Visible = false;
                panel16.BackColor = System.Drawing.Color.Lime;
                panel16.Visible = true;
            
            } 
          
          if(energyNasob > 1 && Abiggerclick == false) {
                Abiggerclick = true;
                achievments += 1;
                gemy += 1;
                panel14.Visible = false;
                panel14.BackColor = System.Drawing.Color.Lime;
                panel14.Visible = true;
            }

          if (maxEnergy > 100 && Abiggercapacity == false) {
                Abiggercapacity = true;
                achievments += 1;
                gemy += 2;
                panel18.Visible = false;
                panel18.BackColor = System.Drawing.Color.Lime;
                panel18.Visible = true;
            }
          //Stovka 
          if (penize >= 100 && Astovka == false) {
                Astovka = true;
                achievments += 1;
                gemy += 2;
                panel20.Visible = false;
                panel20.Size = new Size(87, 29);
                panel20.Visible = true;
            }

          if (Astovka == false)
            {
                panel20.Visible = false;
                panel20.Size = new Size((87*penize/100),29);
                panel20.Visible = true;
            }
          //Tisicovka
          if (penize >= 1000 && Atisicovka == false) {
                Atisicovka = true;
                achievments += 1;
                gemy += 5;
                panel22.Visible = false;
                panel22.Size = new Size(87, 29);
                panel22.Visible = true;
            }

          if (Atisicovka == false)
            {
                panel22.Visible = false;
                panel22.Size = new Size((87 * penize / 1000), 29);
                panel22.Visible = true;
            }
          //Stotisicovka
          if (penize >= 100000 && Astotisicovka == false) {
                Astotisicovka = true;
                achievments += 1;
                gemy += 10;
                panel24.Visible = false;
                panel24.Size = new Size(87, 29);
                panel24.Visible = true;
            }

          if (Astotisicovka == false)
            {
                panel24.Visible = false;
                panel24.Size = new Size((87 * penize / 100000), 29);
                panel24.Visible = true;
            }

          if (maxLastClick > 5 && Alastclickupgrade == false) { 
                Alastclickupgrade = true;
                achievments += 1;
                gemy += 2;
                panel26.Visible = false;
                panel26.BackColor = System.Drawing.Color.Lime;
                panel26.Visible = true;
            }

          if (autoEnergyUpgrade > 0 && Aautoclick == false) {
                Aautoclick = true;
                achievments += 1;
                gemy += 1;
                panel28.Visible = false;
                panel28.BackColor = System.Drawing.Color.Lime;
                panel28.Visible = true;
            }
          //Energy Capacity 
          if (maxEnergy == 500 && Amaxenergy == false) {
                Amaxenergy = true;
                achievments += 1;
                gemy += 5;
                panel30.Visible = false;
                panel30.Size = new Size(87, 29);
                panel30.Visible = true;
            }

          if (Amaxenergy == false)
            {
                panel30.Visible = false;
                panel30.Size = new Size((87*maxEnergy/500),29);
                panel30.Visible = true;
            }
          //Total Clicks
          if (totalClicks >= 1000 && Atisicclicku == false) {
                Atisicclicku = true;
                achievments += 1;
                gemy += 10;
                panel32.Visible = false;
                panel32.Size = new Size(87, 29);
                panel32.Visible = true;
            }

          if (Atisicclicku == false)
            {
                panel32.Visible = false;
                panel32.Size = new Size((87 * totalClicks / 1000), 29);
                panel32.Visible = true;
            }
          //Final
          if (achievments == 14 && Afinale == false) {
                Afinale = true;
                gemy += 1;
                panel34.Visible = false;
                panel34.Size = new Size(474, 29);
                panel34.Visible = true;

                tlacitko.TextAlign = ContentAlignment.TopCenter;
                tlacitko.BackgroundImage = global::Clicker.Properties.Resources._1378582;
                MessageBox.Show("GG Dohrál jsi hru!", "Clicker 0.06", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                panel3.Visible = false;
                panel7.Visible = true;
                panel8.Visible = true;
                update();
            }

          if (Afinale == false)
            {
                panel34.Visible = false;
                panel34.Size = new Size((474 * achievments / 14), 29);
                panel34.Visible = true;
            }
          //Machines
          if (machines == 6  && Amachines == false)
            {
                Amachines = true;
                achievments += 1;
                gemy += 10;
                panel76.Visible = false;
                panel76.Size = new Size(87, 29);
                panel76.Visible = true;
            }
          if (Amachines == false)
            {
               panel76.Visible = false;
               panel76.Size = new Size((87*machines/6),29);
               panel76.Visible = true;
            }
          //Sto surovin
          if(resources >= 100 && AstoResources == false)
            {
                AstoResources = true;
                achievments += 1;
                gemy += 5;
                panel70.Visible = false;
                panel70.Size = new Size(87, 29);
                panel70.Visible = true;
            }
          
          if (AstoResources == false)
            {
                panel70.Visible = false;
                panel70.Size = new Size((87 * resources / 100), 29);
                panel70.Visible = true;
            }

          if(sellRecources > 0 && AsellResource == false) {
                AsellResource = true;
                achievments += 1; 
                gemy += 3;
                panel79.Visible = false;
                panel79.BackColor = System.Drawing.Color.Lime;
                panel79.Visible = true;
            }
          //100Kprodej
          if(sellRecources >= 100000 && A100Kprodej == false)
            {
                A100Kprodej = true;
                achievments += 1;
                gemy += 5;
                panel73.Visible = false;
                panel73.Size = new Size(87, 29);
                panel73.Visible = true;
            }
          if(A100Kprodej == false)
            {
                panel73.Visible = false;
                panel73.Size = new Size((87 * sellRecources / 100000), 29);
                panel73.Visible = true;
            }
        }

        //Prodej

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            //wood
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            //stone
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            //wheat
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            //plank
        }

        private void button21_Click(object sender, EventArgs e)
        {
            //wood
            var number = Convert.ToInt32(numericUpDown1.Value);
            if (wood >= numericUpDown1.Value) 
            {
                wood -= number; 
                penize += 2000 * number; 
                totalPenize += 2000 * number; 
                sellRecources += 2000 * number; 
            }
            updateStorage();

        }

        private void button22_Click(object sender, EventArgs e)
        {
            //stone
            var number = Convert.ToInt32(numericUpDown2.Value);
            if (stone >= numericUpDown2.Value) { stone -= number; penize += 5000*number; totalPenize += 5000 * number; sellRecources += 5000 * number; }
            updateStorage();
        }

        private void button23_Click(object sender, EventArgs e)
        {
            //wheat
            var number = Convert.ToInt32(numericUpDown3.Value);
            if (wheat >= numericUpDown3.Value) { wheat -= number; penize += 500 * number; totalPenize += 500 * number; sellRecources += 500 * number; }
            updateStorage();
        }

        private void button24_Click(object sender, EventArgs e)
        {
            //plank
            var number = Convert.ToInt32(numericUpDown4.Value);
            if (plank >= numericUpDown4.Value) { plank -= number; penize += 10000 * number; totalPenize += 10000 * number; sellRecources += 10000 * number; }
            updateStorage();
        }


        //Tmavý režim:
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
            {
                button19.BackColor = Color.DimGray;
                button19.ForeColor = Color.LightGray;
                autoClick.BackColor = Color.DimGray;
                autoClick.ForeColor = Color.LightGray;
                button3.BackColor = Color.DimGray;
                button3.ForeColor = Color.LightGray;
                tlacitko.BackColor = Color.DimGray;
                tlacitko.ForeColor = Color.LightGray;
                panel1.BackColor = Color.FromArgb(60,60,60);
                
                button4.BackColor = Color.DimGray;
                button4.ForeColor = Color.LightGray;
                panel2.BackColor = Color.FromArgb(60, 60, 60);
                button5.BackColor = Color.DimGray;
                button5.ForeColor = Color.LightGray;
                button7.BackColor = Color.DimGray;
                button7.ForeColor = Color.LightGray;
                button8.BackColor = Color.DimGray;
                button8.ForeColor = Color.LightGray;
                button9.BackColor = Color.DimGray;
                button9.ForeColor = Color.LightGray;
                button10.BackColor = Color.DimGray;
                button10.ForeColor = Color.LightGray;
                button12.BackColor = Color.DimGray;
                button12.ForeColor = Color.LightGray;
                panel6.BackgroundImage = global::Clicker.Properties.Resources.nápis_shop_šedá;
                panel7.BackgroundImage = global::Clicker.Properties.Resources.myš_klik_šedá;
                panel8.BackgroundImage = global::Clicker.Properties.Resources.nápis_clicker_šedá;
                panel2.BackgroundImage = global::Clicker.Properties.Resources.baterka_šedá;
                label2.ForeColor = Color.LightGray;
                panel9.BackgroundImage = global::Clicker.Properties.Resources.dolar_šedá;
                panel11.BackgroundImage = global::Clicker.Properties.Resources.nápis_achievements_šedá;
                panel12.BackgroundImage = global::Clicker.Properties.Resources.achievements_logo_šedá;
                label4.ForeColor = Color.LightGray;
                label5.ForeColor = Color.LightGray;
                label6.ForeColor = Color.LightGray;
                label7.ForeColor = Color.LightGray;
                label8.ForeColor = Color.LightGray;
                label9.ForeColor = Color.LightGray;
                label10.ForeColor = Color.LightGray;
                label11.ForeColor = Color.LightGray;
                label12.ForeColor = Color.LightGray;
                label13.ForeColor = Color.LightGray;
                label14.ForeColor = Color.LightGray;
                label15.ForeColor = Color.LightGray;
                panel43.BackgroundImage = global::Clicker.Properties.Resources.gem_šedá;
                label1.ForeColor = Color.LightGray;
                panel49.BackgroundImage = global::Clicker.Properties.Resources.stats_logo_šedá;
                panel50.BackgroundImage = global::Clicker.Properties.Resources.stats_nápis_šedá;
                panel51.BackColor = Color.LightGray;
                panel52.BackColor = Color.LightGray;


            }

            
            {
                button19.BackColor = Color.Silver;
                button19.ForeColor = Color.Black;
                autoClick.BackColor = Color.Silver;
                autoClick.ForeColor = Color.Black;
                button3.BackColor = Color.Silver;
                button3.ForeColor = Color.Black;
                tlacitko.BackColor = Color.LightGray;
                tlacitko.ForeColor = Color.Black;
                panel1.BackColor = Color.WhiteSmoke;
                button4.BackColor= Color.Silver;
                button4.ForeColor = Color.Black;
                panel2.BackColor = Color.WhiteSmoke;
                button5.BackColor= Color.Silver;
                button5.ForeColor= Color.Black;
                button7.BackColor = Color.Silver;
                button7.ForeColor = Color.Black;
                button8.BackColor = Color.Silver;
                button8.ForeColor = Color.Black;
                button9.BackColor = Color.Silver;
                button9.ForeColor = Color.Black;
                button10.BackColor = Color.Silver;
                button10.ForeColor = Color.Black;
                button12.BackColor = Color.Silver;
                button12.ForeColor = Color.Black;
                panel6.BackgroundImage = global::Clicker.Properties.Resources.nápis_shop_černá;
                panel7.BackgroundImage = global::Clicker.Properties.Resources.myš_klik_černá;
                panel8.BackgroundImage = global::Clicker.Properties.Resources.nápis_clicker_černá;
                panel2.BackgroundImage = global::Clicker.Properties.Resources.baterka_černá;
                label2.ForeColor = Color.Black;
                panel9.BackgroundImage = global::Clicker.Properties.Resources.dolar_černá;
                panel11.BackgroundImage = global::Clicker.Properties.Resources.nápis_achievements_černá;
                panel12.BackgroundImage = global::Clicker.Properties.Resources.achievements_logo_černá;
                label4.ForeColor = Color.Black;
                label5.ForeColor = Color.Black;
                label6.ForeColor = Color.Black;
                label7.ForeColor = Color.Black;
                label8.ForeColor = Color.Black;
                label9.ForeColor = Color.Black;
                label10.ForeColor = Color.Black;
                label11.ForeColor = Color.Black;
                label12.ForeColor = Color.Black;
                label13.ForeColor = Color.Black;
                label14.ForeColor = Color.Black;
                label15.ForeColor = Color.Black;
                panel43.BackgroundImage = global::Clicker.Properties.Resources.gem_černá;
                label1.ForeColor = Color.Black;
                panel49.BackgroundImage = global::Clicker.Properties.Resources.stats_logo_černá;
                panel50.BackgroundImage = global::Clicker.Properties.Resources.stats_nápis_černá;
                panel51.BackColor = Color.Silver;
                panel52.BackColor = Color.Silver;
                
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        //Baterka pozadí - šedé
        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        //Baterka energie - zelené
        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
        //Shop button
        private void button4_Click(object sender, EventArgs e)
        {
            button33.Visible = false;
            button35.Visible = true;
            button4.Visible = false;
            button9.Visible = true;
            button10.Visible = true;
            button8.Visible = true;
            button7.Visible = true;
            button1.Visible = false;
            button6.Visible = false;
            panel84.Visible = false;
            button13.Visible = false;
            label42.Visible = false;

            panel3.Visible = true;
            panel10.Visible = false;
            panel52.Visible = false;
            panel48.Visible = false;
            panel51.Visible = false;
            panel3.Location = new System.Drawing.Point(83, 9);
            update();


        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        //Exit SHop button
        private void button6_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
           
        }

        private void panel3_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void Clicker_Load(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            //Machines button
            button33.Visible = false;
            button35.Visible = true;
            button4.Visible = true;
            button9.Visible = true;
            button10.Visible = true;
            button8.Visible = false;
            button7.Visible = true;
            button1.Visible = false;
            button6.Visible = false;
            panel84.Visible = false;
            button13.Visible = false;
            label42.Visible = false;

            panel51.Visible = true;
            panel10.Visible = false;
            panel3.Visible = false;
            panel52.Visible = false;
            panel48.Visible = false;
            panel51.Location = new System.Drawing.Point(87, 12);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //Storage button
            button33.Visible = false;
            button35.Visible = true;
            button4.Visible = true;
            button9.Visible = true;
            button10.Visible = true;
            button8.Visible = true;
            button7.Visible = false;
            button1.Visible = false;
            button6.Visible = false;
            panel84.Visible = false;
            button13.Visible = false;
            label42.Visible = false;

            panel52.Visible = true;
            panel10.Visible = false;
            panel3.Visible = false;
            panel48.Visible = false;
            panel51.Visible = false;
            panel52.Location = new System.Drawing.Point(83, 7);

            updateStorage();

        }

        private void button9_Click(object sender, EventArgs e)
        {
            //Achievments button
            button33.Visible = false;
            button35.Visible = true;
            button4.Visible = true;
            button9.Visible = false;
            button10.Visible = true;
            button8.Visible = true;
            button7.Visible = true;
            button1.Visible = false;
            button6.Visible = false;
            panel84.Visible = false;
            button13.Visible = false;
            label42.Visible = false;

            panel10.Visible = true;
            panel3.Visible = false;
            panel52.Visible = false;
            panel48.Visible = false;
            panel51.Visible = false;
            panel10.Location = new System.Drawing.Point(83, 12);
            UpdateAchievments();
            Update();
            
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //Stats button
            button33.Visible = false;
            button35.Visible = true;
            button4.Visible = true;
            button9.Visible = true;
            button10.Visible = false;
            button8.Visible = true;
            button7.Visible = true;
            button1.Visible = false;
            button6.Visible = false;
            panel84.Visible = false;
            button13.Visible = false;
            label42.Visible = false;

            panel48.Visible = true;
            panel10.Visible = false;
            panel3.Visible = false;
            panel52.Visible = false;
            panel51.Visible = false;
            panel48.Location = new System.Drawing.Point(89, 14);
            label16.Text = "Total Clicks: "+totalClicks;
            label17.Text = "Total Energy: "+totalEnergy;
            label18.Text = "Total Peněz: "+totalPenize;
            label19.Text = "Počet Machines: " + machines;
            label20.Text = "Double peníze upgrade: "+(nasob-1);
            label21.Text = "Větší klikání upgrade: "+(energyNasob-1);
            label22.Text = "LastClick upgrade: "+(maxLastClick-5);
            label23.Text = "Počet dokončených achievementů: "+achievments;
            label40.Text = "Dohromady surovin: "+resources;
            label41.Text = "Peníze z prodaných surovin: "+sellRecources;
            
        }

        private void panel3_Paint_2(object sender, PaintEventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {
            //logo clickeru ve hře
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {
            //napis cicker ve hře
        }

        private void label1_Click(object sender, EventArgs e)
        {
            //verze
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel10_Paint(object sender, PaintEventArgs e)
        {
            //Achievments
        }

        private void label3_Click(object sender, EventArgs e)
        {
            //napis Achievements ve hře

        }

        private void button13_Click(object sender, EventArgs e)
        {
            //exit Achievements
            panel10.Visible = false;
        }

        private void panel12_Paint(object sender, PaintEventArgs e)
        {
            //logo Achievements

        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            //Vyherni button
           
       
            
        }

        private void panel11_Paint(object sender, PaintEventArgs e)
        {
            //nadpis Achievemets
        }

        private void button14_Click(object sender, EventArgs e)
        {
            
        
        }

        private void panel17_Paint(object sender, PaintEventArgs e)
        {
            //achievement1
        }

        private void panel13_Paint(object sender, PaintEventArgs e)
        {
            //achievement2
        }

        private void label6_Click(object sender, EventArgs e)
        {
            //achievement3 nadpis
        }

        private void panel19_Paint(object sender, PaintEventArgs e)
        {
            //achievement4
        }

        private void label4_Click(object sender, EventArgs e)
        {
            //achievement1 nadpis
        }

        private void label5_Click(object sender, EventArgs e)
        {
            //achievement2 nadpis
        }

        private void label7_Click(object sender, EventArgs e)
        {
            //achievement4 nadpis
        }

        private void panel16_Paint(object sender, PaintEventArgs e)
        {
            //achievement1 progres
        }

        private void panel14_Paint(object sender, PaintEventArgs e)
        {
            //achievement2 progres
        }

        private void panel18_Paint(object sender, PaintEventArgs e)
        {
            //achievement3 progres
        }

        private void panel20_Paint(object sender, PaintEventArgs e)
        {
            //achievement4 progres
        }

        private void panel15_Paint(object sender, PaintEventArgs e)
        {
            //achievement3
        }

        private void panel23_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel21_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel22_Paint(object sender, PaintEventArgs e)
        {
            //achievement5 progres
        }

        private void panel24_Paint(object sender, PaintEventArgs e)
        {
            //achievement6 progres
        }

        private void panel26_Paint(object sender, PaintEventArgs e)
        {
            //achievement7 progres
        }

        private void panel30_Paint(object sender, PaintEventArgs e)
        {
            //achievement9 progres
        }

        private void panel28_Paint(object sender, PaintEventArgs e)
        {
            //achievement8 progres
        }

        private void panel32_Paint(object sender, PaintEventArgs e)
        {
            //achievement10 progres
        }

        private void panel34_Paint(object sender, PaintEventArgs e)
        {
            //achievement11 progres
        }

        private void label14_Click(object sender, EventArgs e)
        {
            
        }

        private void panel41_Paint(object sender, PaintEventArgs e)
        {
            //ukazatel peněz
        }

        private void panel42_Paint(object sender, PaintEventArgs e)
        {
            //ukazatel gemů
        }

        private void label15_Click(object sender, EventArgs e)
        {
            //ukazatel gemů
        }

        private void panel43_Paint(object sender, PaintEventArgs e)
        {
            //logo gemů
        }

        private void panel48_Paint(object sender, PaintEventArgs e)
        {
            //Stats
        }

        private void panel49_Paint(object sender, PaintEventArgs e)
        {
            //logo stats
        }

        private void panel50_Paint(object sender, PaintEventArgs e)
        {
            //nápis stats
        }

        private void label16_Click(object sender, EventArgs e)
        {
            //stat1
        }

        private void label17_Click(object sender, EventArgs e)
        {
            //stat2
        }

        private void label18_Click(object sender, EventArgs e)
        {
            //stat3
        }

        private void label19_Click(object sender, EventArgs e)
        {
            //stat4
        }

        private void label20_Click(object sender, EventArgs e)
        {
            //stat5
        }

        private void label21_Click(object sender, EventArgs e)
        {
            //stat6
        }

        private void label22_Click(object sender, EventArgs e)
        {
            //stat7
        }

        private void label23_Click(object sender, EventArgs e)
        {
            //stat8
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            //exit stats button
            panel48.Visible = false;
        }

        private void button11_Click(object sender, EventArgs e)
        {
           
           
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void button14_Click_1(object sender, EventArgs e)
        {

        }

        private void panel51_Paint(object sender, PaintEventArgs e)
        {
            //Machines
            updateStorage();
        }

        private void button14_Click_2(object sender, EventArgs e)
        {
            //Machines EXit button
            panel51.Visible = false;
        }

        private void panel52_Paint(object sender, PaintEventArgs e)
        {
            //Storage
            updateStorage();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            //Exit Storage button
            panel52.Visible = false;
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void panel53_Paint(object sender, PaintEventArgs e)
        {
            //Machine1
        }

        private void panel54_Paint(object sender, PaintEventArgs e)
        {
            //Machine2
        }

        private void panel55_Paint(object sender, PaintEventArgs e)
        {
            //Machine3
        }

        private void panel58_Paint(object sender, PaintEventArgs e)
        {
            //Machine4
        }

        private void panel57_Paint(object sender, PaintEventArgs e)
        {
            //Machine5
        }

        private void panel56_Paint(object sender, PaintEventArgs e)
        {
            //Machine6
            
        }

        private void label24_Click(object sender, EventArgs e)
        {
            //Machine label 1
        }

        private void label25_Click(object sender, EventArgs e)
        {
            //Machine label 2
        }

        private void label26_Click(object sender, EventArgs e)
        {
            //Machine label 3
        }

        private void label27_Click(object sender, EventArgs e)
        {
            //Machine label 4
        }

        private void label28_Click(object sender, EventArgs e)
        {
            //Machine label 5
        }

        private void label29_Click(object sender, EventArgs e)
        {
            //Machine label 6
        }

        private void panel59_Paint(object sender, PaintEventArgs e)
        {
            //1/2 Shop
        }

        private void panel60_Paint(object sender, PaintEventArgs e)
        {
            //2/2 Shop
        }

        private void label30_Click(object sender, EventArgs e)
        {
            //2/2 Shop nápis
        }

        private void button16_Click(object sender, EventArgs e)
        {

        }

        private void label32_Click(object sender, EventArgs e)
        {

        }

        private void label33_Click(object sender, EventArgs e)
        {

        }

        private void label34_Click(object sender, EventArgs e)
        {

        }

        private void label35_Click(object sender, EventArgs e)
        {

        }

        private void panel64_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel65_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel69_Paint(object sender, PaintEventArgs e)
        {
            //Achievement 
        }

        private void panel72_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel75_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel78_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label36_Click(object sender, EventArgs e)
        {

        }

        private void label37_Click(object sender, EventArgs e)
        {

        }

        private void label38_Click(object sender, EventArgs e)
        {

        }

        private void label39_Click(object sender, EventArgs e)
        {

        }

        private void panel70_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel73_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void panel76_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel79_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel25_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel27_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel29_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label40_Click(object sender, EventArgs e)
        {

        }

        private void label41_Click(object sender, EventArgs e)
        {

        }

        private void button25_Click(object sender, EventArgs e)
        {
            //vyvednout button wood
            
            wood += woodWait;
            woodWait = 0;
            updateStorage();

        }

        private void button26_Click(object sender, EventArgs e)
        {
            stone += stoneWait;
            stoneWait = 0;
            updateStorage();
            //vyvednout button stone
        }

        private void button27_Click(object sender, EventArgs e)
        {
            wheat += wheatWait;
            wheatWait = 0;
            updateStorage();
            //vyvednout button wheet
        }

        private void button28_Click(object sender, EventArgs e)
        {
            plank += plankWait;
            plankWait = 0;
            updateStorage();
            //vyvednout button planks
        }

        private void panel80_Paint(object sender, PaintEventArgs e)
        {
            //nápis storage
        }

        private void label24_Click_1(object sender, EventArgs e)
        {
            //nápis storage
        }

        private void panel82_Paint(object sender, PaintEventArgs e)
        {
            //battery skins panel
        }

        private void label26_Click_1(object sender, EventArgs e)
        {
            //battery skins label
        }

        private void button11_Click_2(object sender, EventArgs e)
        {
            //battery barel skin
            radioButton2.Checked = radioButton2.Checked;
            label27.Visible = true;  
            radioButton1.Visible = true;
            radioButton2.Visible = true;
            if (gemy >= 6)
            { 
                gemy = gemy - 6;
                button11.Visible = false;
            }
        }

        private void button29_Click(object sender, EventArgs e)
        {
            //battery fish skin
            radioButton3.Visible = true;
            if (gemy >= 10)
            {
                gemy = gemy - 10;
                button29.Visible = false;
            }
        }

        private void button30_Click(object sender, EventArgs e)
        {
            //battery coffe skin
            radioButton4.Visible = true;
            if (gemy >= 12)
            {
                gemy = gemy - 12;
                button30.Visible = false;
            }
        }

        private void button31_Click(object sender, EventArgs e)
        {
            //battery ? skin
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //battery skin rb
            panel2.BackgroundImage = global::Clicker.Properties.Resources.baterka_černá;
           

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //battery barrel skin rb
            panel2.BackgroundImage = global::Clicker.Properties.Resources.battery_barrel_skin;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            //battery fish skin rb
            panel2.BackgroundImage = global::Clicker.Properties.Resources.battery_fish_skin;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            //battery coffe skin rb
            panel2.BackgroundImage = global::Clicker.Properties.Resources.battery_coffe_skin;
        }

        private void panel83_Paint(object sender, PaintEventArgs e)
        {
            //battery bacground
        }

        private void panel2_Paint_1(object sender, PaintEventArgs e)
        {
            //baterka skin

        }

        private void label27_Click_1(object sender, EventArgs e)
        {
            //nadpis battery skins
        }

        private void button31_Click_1(object sender, EventArgs e)
        {
            penize = penize + 10000;
            update();
        }

        private void button32_Click(object sender, EventArgs e)
        {
            gemy = gemy + 100;
            update();
        }

        private void button33_Click(object sender, EventArgs e)
        {
            
        }

        private void button34_Click(object sender, EventArgs e)
        {
           


        }

        private void button35_Click(object sender, EventArgs e)
        {
            //tlacitko clicker aplikace
            button33.Visible = true;
            button35.Visible = false;

            button4.Visible = true;
            button9.Visible = true;
            button10.Visible = true;
            button8.Visible = true;
            button7.Visible = true;
            button1.Visible = true;
            if (tlacitko.Visible == false)
                {
                    button6.Visible = true;
                }
            button13.Visible = true;
            if (admin_rezim == true)
            {
                label42.Visible = true;
            }

            panel10.Visible = false;
            panel3.Visible = false;
            panel52.Visible = false;
            panel48.Visible = false;
            panel51.Visible = false;

        }

        void save_question()
        {
            DialogResult dr = MessageBox.Show("Chcete hru uložit?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No)
            {
                Application.Exit();
            }
            else
            {
                SaveData();
                Application.Exit();
            }
        }

        private void button1_Click_3(object sender, EventArgs e)
        {

            if (File.Exists(filePath) == true)
            {
                LoadData();

                if (penize == Int32.Parse(penize_save) && gemy == Int32.Parse(gemy_save))
                {
                    Application.Exit();
                }
                else
                {
                    save_question();
                }
            }
            else
            {
                save_question();
            }

        }

        private async void button6_Click_1(object sender, EventArgs e)
        {
            lastClick = maxLastClick;
            if (energy < maxEnergy)
            {
                energy += baseEnergy * energyNasob;
                totalEnergy += baseEnergy * energyNasob;
                if (energy > maxEnergy) { energy = maxEnergy; }
                totalClicks += 1;
                button6.Visible = false;
                tlacitko.Visible = true;
                update();
            }
            if (checkClicksOn == false)
            {
                checkClicksOn = true;

                button6.Visible = false;
                tlacitko.Visible = true;
                await checkClicks();
            }
            if (penizeGiving == false)
            {
                penizeGiving = true;
                button6.Visible = false;
                tlacitko.Visible = true;
                await penizeGive();
            }
           
        }

        private void label29_Click_1(object sender, EventArgs e)
        {

        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            
            if (panel84.Visible == false && panel85.Visible == false)
            {
                if (admin_rezim == false)
                {
                    panel84.Location = new System.Drawing.Point(287, 12);
                    panel84.Visible = true;
                }

                else if (admin_rezim == true)
                {
                    panel85.Location = new System.Drawing.Point(287, 12);
                    panel85.Visible = true;
                }
               
            }

            else if (panel84.Visible == true || panel85.Visible == true)
            {
                panel84.Visible = false;
                panel85.Visible = false;
            }

           

        }

        private void panel84_Paint(object sender, PaintEventArgs e)
        {
            //admin dialog vstup
        }

        private void button14_Click_3(object sender, EventArgs e)
        {

            if (textBox1.Text == admin_heslo)
            {
                admin_rezim = true;
                button31.Visible = true;
                button32.Visible = true;
                label42.Visible = true;
                panel84.Visible = false;

            }

            else
            {
                textBox1.Text = "";
            }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            //heslo pro přístup admina
        }

        private void label29_Click_2(object sender, EventArgs e)
        {

        }

        private void label42_Click(object sender, EventArgs e)
        {
            //label s nápisem admin mode
        }

        private void panel85_Paint(object sender, PaintEventArgs e)
        {
            //admin dialog exit
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            admin_rezim = false;
            panel85.Visible = false;
            button31.Visible = false;
            button32.Visible = false;
            label42.Visible = false;
        }

        private void label43_Click(object sender, EventArgs e)
        {

        }

        private void button36_Click_1(object sender, EventArgs e)
        {
            panel66.Visible = false;
        }

        private void button37_Click_1(object sender, EventArgs e)
        { //load game button
            if (File.Exists(filePath) == false)
            {
                MessageBox.Show("Nexistuje žádná záloha", "", MessageBoxButtons.OK, MessageBoxIcon.Error);      
            }

            else if (File.Exists(filePath) == true)
            {
                LoadData();

                penize = Int32.Parse(penize_save);
                gemy = Int32.Parse(gemy_save);

                panel66.Visible = false;
                update();
            }
            
        }

        private void panel66_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label32_Click_1(object sender, EventArgs e)
        {

        }

        // Šifrování textu pomocí AES
        private byte[] EncryptData(string data, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);  // 16 znaků pro AES
                aes.IV = new byte[16];  // Inicializační vektor (IV)

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    return encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(data), 0, data.Length);
                }
            }
        }

        // Dešifrování dat pomocí AES
        private string DecryptData(byte[] data, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);  // 16 znaků pro AES
                aes.IV = new byte[16];  // Inicializační vektor (IV)

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(data, 0, data.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        private void button33_Click_1(object sender, EventArgs e)
        {
            //save game button
            SaveData();
        }

        private void SaveData()
        {
            string jsonData = JsonSerializer.Serialize(data);
            string encryptedData = Encrypt(jsonData, key);
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(encryptedData);
            }
        }

        private void LoadData()
        {
            string encryptedData;
            using (StreamReader reader = new StreamReader(filePath))
            {
                encryptedData = reader.ReadToEnd();
            }
            string decryptedData = Decrypt(encryptedData, key);
            data = JsonSerializer.Deserialize<DataContainer>(decryptedData);

        }

        private string Encrypt(string text, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[16]; // Inicializační vektor (pro jednoduchost nulový)
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(text);
                    sw.Flush();
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        private string Decrypt(string encryptedText, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[16];
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(encryptedText)))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private class DataContainer
        {
            public int x { get; set; }
            public int y { get; set; }
            public int z { get; set; }
            public int a { get; set; }
            public int b { get; set; }
        }
    }
}