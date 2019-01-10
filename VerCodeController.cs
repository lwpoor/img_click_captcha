using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Tools;

namespace WAPUI.Controllers
{
    public class VerCodeController : Controller
    {

        public ActionResult Test()
        {
            return View();
        }

        /// <summary>
        /// 根据图片高宽更新文字位置
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public JsonResult UpdateFontPoint(int height, int width)
        {
            if (Session["codepic"] != null && Session["fontSize"]!=null)
            {
                decimal w = (decimal)width / 280;
                decimal h = (decimal)height / 100;
                Session["codepic"] = ((PicPoints.Font1.X * w).ToZMInt32() + "," + (PicPoints.Font1.Y * h).ToZMInt32() + ","
                    + (PicPoints.Font2.X * w).ToZMInt32() + "," + (PicPoints.Font2.Y * h).ToZMInt32() + ","
                    + (PicPoints.Font3.X * w).ToZMInt32() + "," + (PicPoints.Font3.Y * h).ToZMInt32() + ","
                    + (PicPoints.Font4.X * w).ToZMInt32() + "," + (PicPoints.Font4.Y * h).ToZMInt32());
                Session["fontSize"] = Session["fontSize"].ToZMDeciaml() * h;

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 校验图片验证码是否正确
        /// </summary>
        /// <param name="piccode"></param>
        /// <returns></returns>
        public JsonResult CheckPicCode([FromBody]CheckPicCodeViewModel piccode)
        {
            JsonResult<bool> result = new JsonResult<bool>();
            result.code = 1;
            result.msg = "OK";
            if (piccode == null)
            {
                result.Result = false;
                result.ResultMsg = "参数错误";
                return Json(result,JsonRequestBehavior.AllowGet);
            }

            bool flag = false;

            if (Session["codepic"] != null && Session["fontSize"]!=null)
            {
                int fontSize = Session["fontSize"].ToZMInt32();
                string codepic = Session["codepic"].ToString();
                string[] pics = codepic.Split(',');
                var ab1 = Math.Abs(pics[0].ToZMInt32() - piccode.x1);
                var ab2 = Math.Abs(pics[1].ToZMInt32() - piccode.y1);
                var ab3 = Math.Abs(pics[2].ToZMInt32() - piccode.x2);
                var ab4 = Math.Abs(pics[3].ToZMInt32() - piccode.y2);
                var ab5 = Math.Abs(pics[4].ToZMInt32() - piccode.x3);
                var ab6 = Math.Abs(pics[5].ToZMInt32() - piccode.y3);
                var ab7 = Math.Abs(pics[6].ToZMInt32() - piccode.x4);
                var ab8 = Math.Abs(pics[7].ToZMInt32() - piccode.y4);
                if (ab1 > fontSize || ab2 > fontSize
                || ab3 > fontSize || ab4 > fontSize
                || ab5 > fontSize || ab6 > fontSize
                || ab7 > fontSize || ab8 > fontSize)
                {
                    flag = false;
                    result.ResultMsg = "验证码错误";
                }
                else
                {
                    flag = true;
                    result.ResultMsg = "验证码正确";
                }
            }
            else
            {
                flag = false;
                result.ResultMsg = "验证码已过期";
            }
            result.Result = flag;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 校验图片验证码是否正确
        /// </summary>
        public class CheckPicCodeViewModel
        {
            /// <summary>
            /// 客户端令牌
            /// </summary>
            public string token { get; set; }

            public double x1 { get; set; }

            public double x2 { get; set; }

            public double x3 { get; set; }

            public double x4 { get; set; }

            public double y1 { get; set; }

            public double y2 { get; set; }

            public double y3 { get; set; }

            public double y4 { get; set; }
        }

        /// <summary>
        /// 获取验证码，有效时间10分钟
        /// </summary>
        /// <returns></returns>
        public JsonResult VerCodeImg()
        {
            JsonResult<VerCodePicViewModel> result = new JsonResult<VerCodePicViewModel>();
            result.code = 1;
            result.msg = "OK";
            try
            {
                //生成随机的中文验证码
                string yzm = "人口手大小多少上中下男女天地会反清复明杨中科小宝双儿命名空间语现在明天来多个的我山东河北南固安北京南昌东海西安是沙河高教园学"
                    + "木禾上下土个八入大天人火文六七儿九无口日中了子门月不开四五目耳头米见白田电也长山出飞马鸟云公车牛羊小少巾牙尺毛又心手水广升足"
                    + "走方半巴业本平书自已东西回片皮生里果几用鱼今正雨两瓜衣来年左右万百丁齐冬说友话春朋高你绿们花红草爷亲节的岁行古处声知多忙洗真认父扫"
                    + "母爸写全完关家看笑着兴画会妈合奶放午收女气太早去亮和李语秀千香听远唱定连向以更后意主总先起干明赶净同专工才级队蚂蚁前房空网诗黄林闭"
                    + "童立是我朵叶美机她过他时送让吗往吧得虫很河借姐呢呀哪谁凉怕量跟最园脸因阳为光可法石找办许别那到都吓叫再做象点像照沙海桥军竹苗井面乡"
                    + "忘想念王这从进边道贝男原爱虾跑吹乐地老快师短淡对热冷情拉活把种给吃练学习非苦常问伴间共伙汽分要没孩位选北湖南秋江只帮星请雪就球跳玩"
                    + "桃树刚兰座各带坐急名发成动晚新有么在变什条";
                Random r = new Random();
                string validCode = "";
                for (int i = 0; i < 4; i++)
                {
                    int number = r.Next(0, yzm.Length);
                    validCode += yzm[number];
                }


                VerCodePicViewModel vcvm = new VerCodePicViewModel();

                string sourcePic = GetVerCodePicResource();
                if (sourcePic.IsNull() || !System.IO.File.Exists(sourcePic))
                {
                    sourcePic = @"D:\zm\xunguang-4.jpg";
                }
                VerCodePic codepic = GetVerCodePic(validCode, sourcePic);
                vcvm.content = validCode;
                vcvm.MainPic = codepic.PicURL;
                result.Result = vcvm;

                PublicConfig.SetCookie("ValidateCode", validCode, 0, 30, true);
                string token = codepic.Font1.X + "," + codepic.Font1.Y + "," + codepic.Font2.X + "," + codepic.Font2.Y + "," 
                    + codepic.Font3.X + "," + codepic.Font3.Y + "," + codepic.Font4.X + "," + codepic.Font4.Y;
                PublicConfig.SetCookie("__RequestNcoSig", token, 0, 30, true);
                Session["codepic"] = (codepic.Font1.X + "," + codepic.Font1.Y + "," + codepic.Font2.X + "," + codepic.Font2.Y + "," + codepic.Font3.X + "," + codepic.Font3.Y + "," + codepic.Font4.X + "," + codepic.Font4.Y);
                Session["fontSize"] = 20;

                result.ResultMsg = "";
            }
            catch (Exception ex)
            {
                result.code = -1;
                result.msg = "AccountController.VerCodePic发生异常:" + ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取验证码图片资源文件
        /// </summary>
        /// <returns></returns>
        public static string GetVerCodePicResource()
        {
            int i = GetRandom(1, 9);
            return Path.Combine("");
        }
        /// <summary>
        /// 生成指定范围的随机数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandom(int min, int max)
        {
            Random ro = new Random();
            return ro.Next(min, max);
        }

        private static ArrayList _FontPoint;
        public static ArrayList FontPoints
        {
            get
            {
                if (_FontPoint == null)
                {
                    _FontPoint = new ArrayList();
                    for (int x = 0; x < 10; x++)
                    {
                        for (int y = 0; y < 5; y++)
                        {
                            _FontPoint.Add(new FontPoint() { X = x * 28, Y = y * 20 });
                        }
                    }
                }
                return _FontPoint;
            }
        }

        private static VerCodePic PicPoints;
        /// <summary>
        /// 根据文字和图片获取验证码图片
        /// </summary>
        /// <param name="content"></param>
        /// <param name="picFileName"></param>
        /// <returns></returns>
        public static VerCodePic GetVerCodePic(string content, string picFileName, int fontSize = 20)
        {
            
            Bitmap bmp = new Bitmap(picFileName);
            List<int> hlist = new List<int>();
            VerCodePic codepic = new VerCodePic();
            int i = GetRandom(0, FontPoints.Count - 1);
            codepic.Font1 = FontPoints[i] as FontPoint;
            hlist.Add(i);

        A: int i2 = GetRandom(0, FontPoints.Count - 1);
            if (hlist.Contains(i2))
                goto A;
            codepic.Font2 = FontPoints[i2] as FontPoint;
            hlist.Add(i2);

        B: int i3 = GetRandom(0, FontPoints.Count - 1);
            if (hlist.Contains(i3))
                goto B;
            hlist.Add(i3);
            codepic.Font3 = FontPoints[i3] as FontPoint;

        C: int i4 = GetRandom(0, FontPoints.Count - 1);
            if (hlist.Contains(i4))
                goto C;
            hlist.Add(i4);
            codepic.Font4 = FontPoints[i4] as FontPoint;

            PicPoints = codepic;

            string fileName = (content + "-" + picFileName + "-" + i + "|" + i2 + "|" + i3 + "|" + i4).ToEncrypt() + Path.GetExtension(picFileName);
            string dir = Path.Combine("D:/ZM/ZMMVC/WAPUI/", "updata/");
            string filePath = Path.Combine(dir, fileName);
            if (System.IO.File.Exists(filePath))
            {
                codepic.PicURL = string.Format("{0}/{1}/{2}", "http://localhost:55292", "updata", fileName);
                return codepic;
            }
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            Graphics g = Graphics.FromImage(bmp);
            Font font = new Font("微软雅黑", fontSize, GraphicsUnit.Pixel);
            SolidBrush sbrush = new SolidBrush(Color.White);
            SolidBrush sbrush1 = new SolidBrush(Color.White);
            SolidBrush sbrush2 = new SolidBrush(Color.YellowGreen);
            SolidBrush sbrush3 = new SolidBrush(Color.SkyBlue);
            List<char> fontlist = content.ToList();
            g.DrawString(fontlist[0].ToString(), font, sbrush, new PointF(codepic.Font1.X, codepic.Font1.Y));
            g.DrawString(fontlist[1].ToString(), font, sbrush1, new PointF(codepic.Font2.X, codepic.Font2.Y));
            g.DrawString(fontlist[2].ToString(), font, sbrush2, new PointF(codepic.Font3.X, codepic.Font3.Y));
            g.DrawString(fontlist[3].ToString(), font, sbrush3, new PointF(codepic.Font4.X, codepic.Font4.Y));

            bmp.Save(filePath, ImageFormat.Jpeg);
            codepic.PicURL = string.Format("{0}/{1}/{2}", "http://localhost:55292", "updata", fileName);
            return codepic;
        }

        /// <summary>
        /// 二维码图片
        /// </summary>
        public class VerCodePic
        {
            /// <summary>
            /// 图片链接
            /// </summary>
            public string PicURL { get; set; }
            /// <summary>
            /// 第一个字位置
            /// </summary>
            public FontPoint Font1 { get; set; }
            /// <summary>
            /// 第二个字位置
            /// </summary>
            public FontPoint Font2 { get; set; }
            /// <summary>
            /// 第三个字位置
            /// </summary>
            public FontPoint Font3 { get; set; }
            /// <summary>
            /// 第四个字位置
            /// </summary>
            public FontPoint Font4 { get; set; }
        }
        /// <summary>
        /// 文字位置
        /// </summary>
        public class FontPoint
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
        public class JsonResult<T>
        {
            /// <summary>
            /// Json状态码：0.接口逻辑正常但值为空；1.接口逻辑正常，有返回值
            ///             -1.接口异常；-2.签名失败；-3.访问接口超时
            /// </summary>
            public int code { get; set; }
            /// <summary>
            /// Json状态说明
            /// </summary>
            public string msg { get; set; }

            public T Result { get; set; }

            public string ResultMsg { get; set; }
        }
        public class VerCodePicViewModel
        {
            /// <summary>
            /// 图片
            /// </summary>
            public string MainPic { get; set; }
            /// <summary>
            /// 成语内容
            /// </summary>
            public string content { get; set; }
        }


    }
}