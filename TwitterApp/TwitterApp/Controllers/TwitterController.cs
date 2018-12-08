using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwitterApp.DataAccessLayer;
using TwitterApp.ViewModels;

namespace TwitterApp.Controllers
{
    public class TwitterController : Controller
    {
        TwitterRepository repo = new TwitterRepository();        
        // GET: Twitter
        public ActionResult Index()
        {
            Session["User_Id"] = "";
            Session["FullName"] = "";
            Session["Email"] = "";
            ViewBag.PageHeader = "Welcom to My Twitter clone";
            return View();
        }
        public ActionResult Login()
        {
            Session["User_Id"] = "";
            Session["FullName"] = "";
            Session["Email"] = "";
            ViewBag.PageHeader = "Welcom to My Twitter clone";
            return View();
        }
        
        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            ViewBag.PageHeader = "Welcom to My Twitter clone";
            var isValidUser = repo.IsValidLogin(login);
            if (isValidUser)
            {
                var person = repo.GetPerson(login.User_Id);
                Session["User_Id"] = person.User_Id.ToString();
                Session["FullName"] = person.FullName.ToString();
                Session["Email"] = person.Email.ToString();
                return RedirectToAction("TwitterStream");
            }
            else
            {
                ViewBag.Message = "Invalid UserName/Password";
                return View();
            }

        }
        public ActionResult SignUp()
        {
            ViewBag.PageHeader = "Join My Twitter clone";
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(SignUpViewModel model)
        {
            try
            {
                ViewBag.PageHeader = "Join My Twitter clone";
                string userid = repo.AddPerson(model);
                var person = repo.GetPerson(userid);
                Session["User_Id"] = person.User_Id.ToString();
                Session["FullName"] = person.FullName.ToString();
                Session["Email"] = person.Email.ToString();
                return RedirectToAction("TwitterStream");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "SignUp Failed - " + ex.Message;
                return View();
            }
        }
        // GET: Twitter
        public ActionResult TwitterStream()
        {
            ViewBag.PageHeader = "Join My Twitter clone";
            if (!string.IsNullOrEmpty(Session["User_Id"].ToString()))
            {
                var tweets = repo.GetAllTweetsByUser(Session["User_Id"].ToString());
                ViewBag.AllTweets = tweets;
                ViewBag.TweetsCount = tweets.Count() > 0 ? tweets.Count().ToString() : "0";
                ViewBag.currentUserId = Session["User_Id"].ToString();
                ViewBag.FollowerCount = repo.GetFollowersCount(Session["User_Id"].ToString());
                ViewBag.FollowingCount = repo.GetFollowingCount(Session["User_Id"].ToString());
            }
            else
            {
                return RedirectToAction("Index");
            }
            return View(new TweetViewModel());
        }
        [HttpPost]
        public ActionResult TwitterStream(TweetViewModel model)
        {
            try
            {
                ViewBag.PageHeader = "Join My Twitter clone";
                model.User_Id = Session["User_Id"].ToString();
                model.Email = Session["Email"].ToString();
                string userid = repo.AddTweet(model);
                var tweets = repo.GetAllTweetsByUser(userid);
                ViewBag.currentUserId = userid;
                ViewBag.AllTweets = tweets;
                ViewBag.TweetsCount = tweets.Count() > 0 ? tweets.Count().ToString() : "";
                ViewBag.FollowerCount = repo.GetFollowersCount(userid);
                ViewBag.FollowingCount = repo.GetFollowingCount(userid);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Failed - " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult SaveTweet(int tweetId, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (ModelState.IsValid)
                {
                    var model = new TweetViewModel();
                    model.User_Id = Session["User_Id"].ToString();
                    model.Email = Session["Email"].ToString();
                    model.Message = message;
                    model.Tweet_Id = tweetId;
                    string userid = tweetId > 0 ? repo.EditTweet(model) : repo.AddTweet(model);
                    return Json(new { success = true });
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteTweet(int tweetId)
        {
            try
            {
                string userid = repo.DeleteTweet(tweetId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public ActionResult UserProfile()
        {
            ViewBag.PageHeader = "My Twitter clone";
            if (!string.IsNullOrEmpty(Session["User_Id"].ToString()))
            {
                var userProfile = repo.GetPerson(Session["User_Id"].ToString());
                return View(userProfile);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
        // GET: Twitter/Details/5
        public ActionResult EditUserProfile(string userId)
        {
            ViewBag.PageHeader = "My Twitter clone";
            return View(repo.GetPerson(userId));
        }
        [HttpPost]
        public ActionResult EditUserProfile(SignUpViewModel model)
        {
            try
            {
                ViewBag.PageHeader = "My Twitter clone";
                string userid = repo.UpdatePerson(model);
                var person = repo.GetPerson(userid);
                Session["User_Id"] = person.User_Id.ToString();
                Session["FullName"] = person.FullName.ToString();
                Session["Email"] = person.Email.ToString();
                return RedirectToAction("TwitterStream");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Profile Update Failed - " + ex.Message;
                return View();
            }
        }

        // GET: Twitter/Create
        public ActionResult OtherUserProfile(string userId)
        {
            ViewBag.PageHeader = "My Twitter clone";
            if (!string.IsNullOrEmpty(userId))
            {
                var userProfile = repo.GetPerson(userId);
                string currentUser = Session["User_Id"].ToString();
                var followResult = repo.GetFollowDetails(currentUser, userId);
                ViewBag.FollowDetails = followResult > 0? "Unfollow" : "Follow";
                ViewBag.otherUser = userId;
                return View(userProfile);
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult FollowOrUnFollw(bool followValue, string otherUserId)
        {
            try
            {
                string currentUser = Session["User_Id"].ToString();
                int result = repo.SaveFollowOrUnfollowDetails(currentUser, otherUserId, followValue);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        // POST: Twitter/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public JsonResult SearchPerson(string keyword)
        {
            var allPersons = repo.GetAllPersons();
            var matchedResult = from p in allPersons
                                where p.FullName.StartsWith(keyword)
                                select new { UserId = p.User_Id, Name = p.FullName };
            return Json(matchedResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteAccount(string userId)
        {
            var result = repo.DeleteTwitterAccount(userId);           
            ViewBag.PageHeader = "Welcom to My Twitter clone";
            return RedirectToAction("Login");
        }
    }
}
