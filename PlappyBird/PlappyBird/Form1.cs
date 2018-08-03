using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Media;
using PlappyBird.Properties;
using System.Reflection;
using System.Runtime.InteropServices;

// Thực hành coding covention và readable code.
namespace PlappyBird
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Khai bao list ống 1.
        // Pipe1 -> pipe1.
        List<int> pipe1 = new List<int>();

        // Khai bao list ống 2.
        // Pipe2 -> pipe2.
        List<int> pipe2 = new List<int>();

        // Độ rống của ống là 55.
        // PipeWidth -> pipeWidth.
        int pipeWidth = 55;

        // Độ rộng giữa ống trên và ống dưới.
        // PipeDiffrentY -> pipeDiffrentY.
        int pipeDiffrentY = 200;

        // Độ rộng giữa 2 ống.
        // PipeDifferentX -> pipeDifferentX.
        int pipeDifferentX = 300;

        bool start = true;

        bool running;

        // Khởi tạo khi con chim chưa chơi có độ bay là 0.
        int step = 0;

        // vị trí của con chim.
        // Originalx -> originalX.
        // Originaly -> originalY.
        int originalX, Originaly;
        
        // ResetPipes -> resetPipes
        bool resetPipes = false;

        int points;

        bool inPipe = false;

        int score;

        // scorediffrent -> scoreDifferent.
        int scoreDifferent;

        // Show điểm của player khi chim die.
        // ReadAndShowScore -> readAndShowScore (Method).
        private void readAndShowScore()
        {
            using (StreamReader reader = new StreamReader("Score.ini")) {

                score = int.Parse(reader.ReadToEnd());
                reader.Close();

                if (int.Parse(label1.Text) == 0 | int.Parse(label1.Text) > 0) {
                    scoreDifferent = score - int.Parse(label1.Text) + 1;
                }

                if (score < int.Parse(label1.Text)) {
                    MessageBox.Show(string.Format("Điểm cao nhất hiện tại là {0}. Bạn đã vượt qua mức đó. Xin chúc mừng. Điểm số mới hiện tại l", score, label1.Text), "Flappy Bird", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    using (StreamWriter writer = new StreamWriter("Score.ini")) {
                        writer.Write(label1.Text);
                        writer.Close();
                    }
                }

                if (score > int.Parse(label1.Text)) {
                    MessageBox.Show(string.Format("Bạn cần {0} điểm để có được điểm số cao nhất {1}", scoreDifferent, score), "Flappy Bird", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (score == int.Parse(label1.Text)) {
                    MessageBox.Show(string.Format("Bạn đã thực hiện chính xác {0} (điểm tối đa). Cố gắng đánh bại nó lần này.", score), "Flappy Bird", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // StartGame -> startGame (Method)
        private void startGame()
        {
            // Reset con chim = false;
            resetPipes = false;

            // chạy 3 hàm timer
            timer1.Enabled = true;

            // vễ ống và xét va chạm.
            timer2.Enabled = true;

            // làm cho con chim di chuyển
            timer3.Enabled = true;

            // khởi tạo biến random.
            Random random = new Random();

            // random ống trên thứ 1.
            int numPipe1UP = random.Next(40, this.Height - this.pipeDiffrentY);

            // random ống dưới thứ 1.
            int numPipe1Down = numPipe1UP + this.pipeDiffrentY;

            // add vào ống to trên.
            pipe1.Add(this.Width);

            // add vào nhỏ trên
            pipe1.Add(numPipe1UP);

            // add vào ống to dưới.
            pipe1.Add(this.Width);

            // add vào ổng nhỏ dưới.
            pipe1.Add(numPipe1Down);

            // random ống trên thứ 2.
            numPipe1UP = random.Next(40, (this.Height - pipeDiffrentY));

            // random ống dưới thứ 2.
            numPipe1Down = numPipe1UP + pipeDiffrentY;
            pipe2.Add(this.Width + pipeDifferentX);
            pipe2.Add(numPipe1UP);
            pipe2.Add(this.Width + pipeDifferentX);
            pipe2.Add(numPipe1Down);

            // khi game chạy thì ẩn button 1
            button1.Visible = false;
            button1.Enabled = false;
            running = true;
        }


        // chết.
        // Die -> die (Method)
        private void die()
        {
            // dừng chạy con chim.
            running = false;

            // dừng timer 2 và 3.
            timer2.Enabled = false;
            timer3.Enabled = false;

            // hiện nút button1 .
            button1.Visible = true;
            button1.Enabled = true;

            // vào hàm đọc
            readAndShowScore();

            // reset points = 0
            points = 0;

            // đưa hình ảnh về vị trí ban đầu
            pictureBox1.Location = new Point(originalX, Originaly);

            // không cho hìn chuyển động
            resetPipes = true;

            // xóa 2 ống.
            pipe1.Clear();
            pipe2.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // vị trí đâu tiên của con chim.
            originalX = pictureBox1.Location.X;
            Originaly = pictureBox1.Location.Y;

            // khởi tạo file Score
            if (!File.Exists("Score.ini")) {
                File.Create("Score.ini").Dispose();
            }
        }
        
        // Vẽ lại ống.
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        // bất đầu game.
        private void button1_Click(object sender, EventArgs e)
        {
            startGame();
        }

        // Xóa ống khi load hết màn hình.
        private void timer2_Tick(object sender, EventArgs e)
        {
            // ống 1. nếu xóa game chỉ còn 1 ống.
            // Xét ống dưới có bị khuất hay không. chạy về màn hình.
            if (pipe1[0] + pipeWidth <= 0) {
                // khởi tạo random
                Random rnd = new Random();
                int px = this.Width;
                int py = rnd.Next(40, (this.Height - pipeDiffrentY));
                var p2x = px;
                var p2y = py + pipeDiffrentY;

                // không clear sẽ không khởi tạo trong lần chim bay qua ống thứ 2
                pipe1.Clear();
                pipe1.Add(px);
                pipe1.Add(py);
                pipe1.Add(p2x);
                pipe1.Add(p2y);
            } else {
                // nếu bỏ 2 thằng này game chỉ còn 1 ống. xét khi nó chưa trôi hết màn hình.
                pipe1[0] = pipe1[0] - 2;
                pipe1[2] = pipe1[2] - 2;
            }

            // ống 2. nếu xóa game chỉ còn 1 ống
            if (pipe2[0] + pipeWidth <= 0) {
                Random rnd = new Random();
                int px = this.Width;
                int py = rnd.Next(40, (this.Height - pipeDiffrentY));
                var p2x = px;
                var p2y = py + pipeDiffrentY;

                // không clear sẽ không khởi tạo ống thứ 2.
                pipe2.Clear();
                pipe2.Add(px);
                pipe2.Add(py);
                pipe2.Add(p2x);
                pipe2.Add(p2y);
            } else {
                pipe2[0] = pipe2[0] - 2;
                pipe2[2] = pipe2[2] - 2;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // nếu reset ống = false. Hiện ra ống 1 và ống 2.
            if (resetPipes == false && pipe1.Any() && pipe2.Any()) {
                // ống 1 phía tren nằm ở vị trị đầu tiền.(vị trí đầu tiên khi xuất hiện ống, vị trí từ số 0 phía trên màn hình, độ rộng của ống là 55, chiều cao)
                e.Graphics.FillRectangle(Brushes.DarkGreen, new Rectangle(pipe1[0], 0, 55, pipe1[1]));
                // Vẽ gạch phía trên.(vị trí gạch phía dưới theo truc x, vị trí gạch phía dưới theo trục y, chiều dài, chiều ngang)
                e.Graphics.FillRectangle(Brushes.DarkGreen, new Rectangle(pipe1[0] - 10, pipe1[3] - pipeDiffrentY, 75, 15));
                // vẽ ống 1 phía dưới ở vị trí cuối màn hình.
                e.Graphics.FillRectangle(Brushes.DarkGreen, new Rectangle(pipe1[2], pipe1[3], 55, this.Height - pipe1[3]));
                // vẽ gạch ngang phía dưới.
                e.Graphics.FillRectangle(Brushes.DarkGreen, new Rectangle(pipe1[2] - 10, pipe1[3], 75, 15));
                // vẽ ống thứ 2.
                e.Graphics.FillRectangle(Brushes.DarkGreen, new Rectangle(pipe2[0], 0, 55, pipe2[1]));
                e.Graphics.FillRectangle(Brushes.DarkGreen, new Rectangle(pipe2[0] - 10, pipe2[3] - pipeDiffrentY, 75, 15));
                // vẽ ống thứ 2.
                e.Graphics.FillRectangle(Brushes.DarkGreen, new Rectangle(pipe2[2], pipe2[3], 55, this.Height - pipe2[3]));
                e.Graphics.FillRectangle(Brushes.DarkGreen, new Rectangle(pipe2[2] - 10, pipe2[3], 75, 15));
            }
        }

        // xác định con chim đi qua giữa 2 cái ống và tăng điểm lên.
        private void checkForPiont()
        {
            Rectangle rec = pictureBox1.Bounds;
            // khoảng cách giữa 2 cột thứ 1
            Rectangle rec1 = new Rectangle(pipe1[2], pipe1[3] - pipeDiffrentY, 15, pipeDiffrentY);
            // khoảng cách giữa 2 cột thứ 2
            Rectangle rec2 = new Rectangle(pipe2[2], pipe2[3] - pipeDiffrentY, 15, pipeDiffrentY);
            // xét con chim có chạy vào giữa đó hay không.
            Rectangle intersect1 = Rectangle.Intersect(rec, rec1);
            Rectangle intersect2 = Rectangle.Intersect(rec, rec2);
            if (!resetPipes | start) {
                if (intersect1 != Rectangle.Empty || intersect2 != Rectangle.Empty) {
                    if (!inPipe) {
                        points++;
                        SoundPlayer sp = new SoundPlayer(PlappyBird.Properties.Resources.point);
                        sp.Play();
                        inPipe = true;
                    }
                } else {
                    inPipe = false;
                }
            }
        }
        // kiểm tra sự va cham.
        private void checkForCollisim()
        {
            Rectangle rec = pictureBox1.Bounds;
            // xét con chim đụng vạch phía trên.(vị trí 0 -> vị trí gạch ngang thay đổi vị trí số 4)
            Rectangle rec1 = new Rectangle(pipe1[0], 0, 55, pipe1[1] + 15);
            // xét cho ống số 1 phía dưới.(thay đổi ở vị trí số 2)
            Rectangle rec2 = new Rectangle(pipe1[2], pipe1[3], 55, this.Height - pipe1[3]);
            Rectangle rec3 = new Rectangle(pipe2[0], 0, 55, pipe2[1] + 15);
            Rectangle rec4 = new Rectangle(pipe2[2], pipe2[3], 55, this.Height - pipe2[3]);
            Rectangle intersect1 = Rectangle.Intersect(rec, rec1);
            Rectangle intersect2 = Rectangle.Intersect(rec, rec2);
            Rectangle intersect3 = Rectangle.Intersect(rec, rec3);
            Rectangle intersect4 = Rectangle.Intersect(rec, rec4);
            if (!resetPipes | start) {
                if (intersect1 != Rectangle.Empty | intersect2 != Rectangle.Empty | intersect3 != Rectangle.Empty | intersect4 != Rectangle.Empty) {
                    SoundPlayer sp = new SoundPlayer(PlappyBird.Properties.Resources.collision);
                    sp.Play();
                    die();
                }
            }
        }

        // Phím nhả ra là Space.
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) {
                case Keys.Space:
                    // chim giảm xuống 5.
                    step = -5;
                    pictureBox1.Image = Resources.bird_straight;
                    break;
            }
        }

        // dừng game khi con chim đụng ống cống.
        private void timer3_Tick(object sender, EventArgs e)
        {
            pictureBox1.Location = new Point(pictureBox1.Location.X, pictureBox1.Location.Y + step);
            if (pictureBox1.Location.Y < 0) {
                pictureBox1.Location = new Point(pictureBox1.Location.X, 0);
            }
            if (pictureBox1.Location.Y + pictureBox1.Height > this.ClientSize.Height) {
                pictureBox1.Location = new Point(pictureBox1.Location.X, this.ClientSize.Height - pictureBox1.Height);
            }
            // tắt chim sẽ bất tử
            checkForCollisim();
            if (running) {
                checkForPiont();
            }
            label1.Text = Convert.ToString(points);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        // Phím ấn xuống là Space
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) {
                case Keys.Space:
                    // chim bay lên 15.
                    step = 5;
                    pictureBox1.Image = Resources.bird_down;
                    break;
            }
        }
    }
}
