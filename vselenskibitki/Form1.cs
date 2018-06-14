using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;

namespace VselenskiBitki
{
    public partial class Form1 : Form
    {
        PlayerSpaceship playerSpaceship;
        List<EnemySpaceship> enemies;
        List<Weapon1> enemiesWeapon;
        Boss boss;
        Image img;
        Image img2;
        private bool isplayable = false;
        private bool BossFight = false;
        private int brojac = 0;
        private int brojac2 = 0;

        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;
            newGame();
        }

        private void SpawnEnemies()
        {
            for (int i = 120; i < 751; i += 100)
                for (int j = 10; j < 300; j += 100)
                {
                    enemies.Add(new EnemySpaceship(i, j));
                }

        }

        public void newGame()
        {
            isplayable = false;
            BossFight = false;
            pbBossLife.Visible = false;
            img = VselenskiBitki.Properties.Resources.redfighter0005;
            img2 = VselenskiBitki.Properties.Resources.lost;
            pictureBox1.Image = img;
            pictureBox2.Image = img;
            pictureBox3.Image = img;
            pictureBox4.Image = img2;
            playerSpaceship = new PlayerSpaceship(PlayerSpaceship.Pictures.normal);
            enemies = new List<EnemySpaceship>();
            enemiesWeapon = new List<Weapon1>();
            SpawnEnemies();
            boss = new Boss(350, 30);
            pbBossLife.Value = boss.life;
            GlobalTimer.Start();
        }

        public void Draw(Graphics g)
        {
            Image img = VselenskiBitki.Properties.Resources.bg1;
            g.DrawImage(img, 1000, 649, img.Width, img.Height);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isplayable)
            {
                if (e.KeyCode == Keys.S)
                {
                    SaveFile(); //sejv
                }
                if (e.KeyCode == Keys.O)
                {
                    OpenFile(); 
                }

                if (e.KeyCode == Keys.Left)
                {
                    if (playerSpaceship.X > 0)
                    {
                        playerSpaceship.ChangePicture(PlayerSpaceship.Pictures.left);
                        playerSpaceship.Move(-10, 0);
                    }
                }
                else if (e.KeyCode == Keys.Right)
                {
                    if (playerSpaceship.X < 920)
                    {
                        playerSpaceship.ChangePicture(PlayerSpaceship.Pictures.right);
                        playerSpaceship.Move(10, 0);
                    }
                }
                Invalidate();
            }
        }

        public void MoveTheWeapon1()
        {
            for (int i = 0; i < playerSpaceship.getWeapon1().Count; i++)
            {
                playerSpaceship.MoveTheWeapon(i);

                if (playerSpaceship.getWeapon1()[i].Y < 0)
                {
                    playerSpaceship.RemoveWeapon1(i);
                    i--;
                    continue;
                }

                Rectangle rec = new Rectangle(playerSpaceship.getWeapon1()[i].X, playerSpaceship.getWeapon1()[i].Y, playerSpaceship.getWeapon1()[i].weapon.Width, playerSpaceship.getWeapon1()[i].weapon.Height);
                for (int j = 0; j < enemies.Count; j++)
                {
                    if (enemies[j].isDestroyed(rec))
                    {
                        playerSpaceship.RemoveWeapon1(i);
                        enemies.Remove(enemies[j]);
                        i--;
                        break;
                    }

                }
                if (BossFight)
                {
                    if (boss.GetsHit(rec) && boss.life > 0)
                    {
                        playerSpaceship.RemoveWeapon1(i);
                        i--;
                        boss.life--;
                        pbBossLife.Value = boss.life;
                    }
                }

            }
        }
        public void moveEnemyWeapon1()
        {            
            if (enemiesWeapon.Count > -1)
            {
                for (int i = 0; i < enemiesWeapon.Count; i++)
                {
                    enemiesWeapon[i].moveWeapon1ForEnemy();
                    if (enemiesWeapon[i].Y > 650)
                    {
                        enemiesWeapon.Remove(enemiesWeapon[i]);
                        i--;
                        continue;
                    }
                    Rectangle rec = new Rectangle(enemiesWeapon[i].X, enemiesWeapon[i].Y, enemiesWeapon[i].weapon2.Width, enemiesWeapon[i].weapon2.Height);
                    if (playerSpaceship.GetHit(rec))
                    {
                        playerSpaceship.LoseLife();
                        GlobalTimer.Stop();
                        ifHit.Start();
                        isplayable = true;
                        pictureBox4.Visible = true;
                        if (playerSpaceship.life == 2) pictureBox3.Image = null;
                        if (playerSpaceship.life == 1) pictureBox2.Image = null;
                        if (playerSpaceship.life == 0) pictureBox1.Image = null;
                        enemiesWeapon.Remove(enemiesWeapon[i]);
                        i++;
                        break;
                    }
                }
            }
        }
        public void moveBossWeapon()
        {
            for (int i = 0; i < boss.getBossWeapon().Count; i++)
            {
                boss.MoveTheBossWeapon(i);

                if (boss.getBossWeapon()[i].Y > 650)
                {
                    boss.RemoveBossWeapon(i);
                    i--;
                    continue;
                }
                Rectangle rec = new Rectangle(boss.getBossWeapon()[i].X, boss.getBossWeapon()[i].Y, boss.getBossWeapon()[i].bossWeapon.Width, boss.getBossWeapon()[i].bossWeapon.Height);
                if (playerSpaceship.GetHit(rec))
                {
                    playerSpaceship.LoseLife();
                    GlobalTimer.Stop();
                    ifHit.Start();
                    isplayable = true;
                    pictureBox4.Visible = true;
                    if (playerSpaceship.life == 2) pictureBox3.Image = null;
                    if (playerSpaceship.life == 1) pictureBox2.Image = null;
                    if (playerSpaceship.life == 0) pictureBox1.Image = null;
                    boss.RemoveBossWeapon(i);
                    i++;
                    break;
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (EnemySpaceship es in enemies)
            {
                es.Draw(e.Graphics);
            }
            foreach (Weapon1 w in playerSpaceship.getWeapon1())
            {
                w.Draw(e.Graphics);

            }
            if (enemiesWeapon != null && enemiesWeapon.Count > 0)
            {
                foreach (Weapon1 w in enemiesWeapon)
                {
                    w.Draw(e.Graphics);
                }
            }
            playerSpaceship.Draw(e.Graphics);
            if (BossFight)
            {
                boss.Draw(e.Graphics);
                foreach (BossWeapon bw in boss.getBossWeapon())
                {
                    bw.Draw(e.Graphics);
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            playerSpaceship.ChangePicture(PlayerSpaceship.Pictures.normal);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!isplayable)
            {
                if (e.KeyChar == (char)Keys.Space)
                {
                    playerSpaceship.AddWeapon1();
                }
                if (e.KeyChar == (char)Keys.Enter)
                {
                    enemies[0].AddWeaponToEnemies();
                }
                Invalidate();
            }
        }

        public void moveTheEnemies()
        {
            foreach (EnemySpaceship es in enemies)
            {
                if (brojac < 12)
                {
                    es.X += 7;
                }
                else if (brojac >= 12 && brojac < 23)
                {
                    es.X += -7;
                }

                if (brojac == 4) es.Y += 10;
                if (brojac == 9) es.Y -= 10;
                if (brojac == 12) es.Y += 10;
                if (brojac == 16) es.Y -= 10;
            }
            if (brojac == 23)
                brojac = 0;
            brojac++;
        }

        private void GlobalTimer_Tick(object sender, EventArgs e)
        {
            if (playerSpaceship.life == 0)
            {
                GlobalTimer.Stop();
                isplayable = true;
                if (MessageBox.Show("Ги изгуби сите животи", "Сакаш да пробаш повторно?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    newGame();
                }
                else
                {
                    Close();
                }
            }
            Random r = new Random();
            moveTheEnemies();

            if (BossFight)
            {
                enemiesWeapon = null;
                boss.BossMoving();
                moveBossWeapon();
                if (r.Next(20) == 10)
                {
                    if (r.Next() % 2 == 0)
                    {
                        boss.FireWeaponLeft();
                    }
                    else boss.FireWeaponRight();
                }
            }
            else
            {
                moveEnemyWeapon1();
                if (r.Next(10) == 5)
                {
                    int rand = r.Next(enemies.Count);
                    int Xx = enemies[rand].X;
                    int Yy = enemies[rand].Y;
                    enemiesWeapon.Add(new Weapon1(Xx + 34, Yy + 80, false));
                }
            }
            if (boss.life == 1)
            {
                GlobalTimer.Stop();
                if (MessageBox.Show("Сакаш нова игра?", "ПОБЕДА!", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    newGame();
                }
                else
                {
                    Close();
                }
            }

            MoveTheWeapon1();
            Invalidate();

            if (enemies.Count == 0)
            {
                if (BossFight != true)
                {
                    pictureBox3.Image = null;
                    pictureBox2.Image = null;
                    playerSpaceship.life=1;// eden zivot ima kaj shefot
                    
                }
                BossFight = true;
                pbBossLife.Visible = true;
            }

        }

        private void ifHit_Tick(object sender, EventArgs e)
        {
            if (brojac2 == 2)
            {
                GlobalTimer.Start();
                ifHit.Stop();
                brojac2 = 0;
                isplayable = false;
                pictureBox4.Visible = false;
            }
            brojac2++;
        }

        public String FileName = "sejv";
        public void SaveFile()
        {
            System.Runtime.Serialization.IFormatter fmt = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.FileStream strm = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None);
            fmt.Serialize(strm, playerSpaceship);
            fmt.Serialize(strm, enemies);
            if (enemiesWeapon != null)
                fmt.Serialize(strm, enemiesWeapon);
            fmt.Serialize(strm, boss);
            fmt.Serialize(strm, img);
            fmt.Serialize(strm, img2);
            fmt.Serialize(strm, isplayable);
            fmt.Serialize(strm, BossFight);
            fmt.Serialize(strm, brojac);
            fmt.Serialize(strm, brojac2);
            strm.Close();
        }
        public void OpenFile()
        {
            try
            {
                System.Runtime.Serialization.IFormatter fmt = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                System.IO.FileStream strm = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
                playerSpaceship = (PlayerSpaceship)fmt.Deserialize(strm);
                enemies = (List<EnemySpaceship>)fmt.Deserialize(strm);
                if(enemies.Count!=0)
                enemiesWeapon = (List<Weapon1>)fmt.Deserialize(strm);
                boss = (Boss)fmt.Deserialize(strm);
                img = (Image)fmt.Deserialize(strm);
                img2 = (Image)fmt.Deserialize(strm);
                isplayable = (bool)fmt.Deserialize(strm);
                BossFight = (bool)fmt.Deserialize(strm);
                brojac = (int)fmt.Deserialize(strm);
                brojac2 = (int)fmt.Deserialize(strm);
                strm.Close();
                if(!BossFight)
                    pbBossLife.Visible = false;
                if (playerSpaceship.life==3)
                {
                    pictureBox1.Image = img;
                    pictureBox2.Image = img;
                    pictureBox3.Image = img;
                }
                else if(playerSpaceship.life == 2)
                {
                    pictureBox3.Image = null;
                    pictureBox2.Image = img;
                    pictureBox1.Image = img;
                }
                else
                {
                    pictureBox3.Image = null;
                    pictureBox2.Image = null;
                    pictureBox1.Image = img;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file \"" + FileName + "\" from disk. Original error: " + ex.Message);              
            }


        }
    }
}