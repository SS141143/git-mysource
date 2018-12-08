using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TwitterApp.ViewModels;

namespace TwitterApp.DataAccessLayer
{
    public class TwitterRepository
    {
        string connString = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
        public string AddPerson(SignUpViewModel model)
        {
            string returnValue = string.Empty;
            try
            {
                using (TwitterDatabaseEntities entity = new TwitterDatabaseEntities())
                {
                    var person = new Person();
                    person.User_Id = model.User_Id;
                    person.Password = Cryptography.Encrypt(model.Password);
                    person.FullName = model.FullName;
                    person.Email = model.Email;
                    person.Joined = DateTime.Now;
                    person.Active = true;
                    entity.People.Add(person);
                    entity.SaveChanges();
                    returnValue = model.User_Id;
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public string UpdatePerson(SignUpViewModel model)
        {
            string returnValue = string.Empty;
            try
            {
                using (TwitterDatabaseEntities entity = new TwitterDatabaseEntities())
                {
                    var person = entity.People.Find(model.User_Id);
                    person.FullName = model.FullName;
                    person.Password = Cryptography.Encrypt(model.Password);
                    person.Email = model.Email;
                    person.Joined = DateTime.Now;
                    person.Active = true;
                    entity.Entry(person).State = EntityState.Modified;
                    entity.SaveChanges();
                    returnValue = model.User_Id;
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool IsValidLogin(LoginViewModel model)
        {
            try
            {
                using (TwitterDatabaseEntities entity = new TwitterDatabaseEntities())
                {
                    var person = entity.People.Find(model.User_Id);
                    if (person != null)
                    {
                        var password = Cryptography.Decrypt(person.Password);
                        bool isValidUser = entity.People.Any(p => p.User_Id.Equals(model.User_Id) && password.Equals(model.Password));
                        return isValidUser;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public SignUpViewModel GetPerson(string user_Id)
        {
            SignUpViewModel model = new SignUpViewModel();
            using (TwitterDatabaseEntities entity = new TwitterDatabaseEntities())
            {
                var dbPerson = entity.People.Find(user_Id);
                model.User_Id = dbPerson.User_Id;
                model.Password = Cryptography.Decrypt(dbPerson.Password);
                model.FullName = dbPerson.FullName;
                model.Joined = dbPerson.Joined;
                model.Email = dbPerson.Email;
                return model;
            }
        }
        public List<SignUpViewModel> GetAllPersons()
        {
            List<SignUpViewModel> allPersons = new List<SignUpViewModel>();
            using (TwitterDatabaseEntities entity = new TwitterDatabaseEntities())
            {
                var dbPersonList = entity.People.ToList();
                foreach (var item in dbPersonList)
                {
                    SignUpViewModel model = new SignUpViewModel();
                    model.User_Id = item.User_Id;
                    model.FullName = item.FullName;
                    model.Joined = item.Joined;
                    model.Email = item.Email;
                    allPersons.Add(model);
                }
                return allPersons;
            }
        }
        public string AddTweet(TweetViewModel model)
        {
            string returnValue = string.Empty;
            try
            {
                using (TwitterDatabaseEntities entity = new TwitterDatabaseEntities())
                {
                    var tweet = new Tweet();
                    tweet.User_Id = model.User_Id;
                    tweet.Message = model.Message;
                    tweet.Email = model.Email;
                    tweet.Created = DateTime.Now;
                    entity.Tweets.Add(tweet);
                    entity.SaveChanges();
                    returnValue = model.User_Id;
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public string EditTweet(TweetViewModel model)
        {
            string returnValue = string.Empty;
            try
            {
                using (TwitterDatabaseEntities entity = new TwitterDatabaseEntities())
                {
                    var tweet = entity.Tweets.Find(model.Tweet_Id);
                    tweet.Message = model.Message;
                    tweet.Email = model.Email;
                    tweet.User_Id = model.User_Id;
                    tweet.Created = DateTime.Now;
                    entity.Entry(tweet).State = EntityState.Modified;
                    entity.SaveChanges();
                    returnValue = model.User_Id;
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public string DeleteTweet(int tweetId)
        {
            string returnValue = string.Empty;
            try
            {
                using (TwitterDatabaseEntities entity = new TwitterDatabaseEntities())
                {
                    var tweet = entity.Tweets.Find(tweetId);
                    entity.Tweets.Remove(tweet);
                    entity.SaveChanges();
                    returnValue = tweet.User_Id;
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public int GetFollowDetails(string currentUser, string otherUser)
        {
            int result = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    string query = string.Format("SELECT CASE WHEN COUNT(1) > 0 THEN 1 ELSE 0 END AS Result FROM [Following] WHERE [User_Id] = '{0}' AND [Following_Id] = '{1}'", currentUser, otherUser);
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.Text;
                        result = (int)cmd.ExecuteScalar();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        public int SaveFollowOrUnfollowDetails(string currentUser, string otherUser, bool followValue)
        {
            int result = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    string follow = string.Format("INSERT INTO [Following] VALUES('{0}','{1}')", currentUser, otherUser);
                    string unfollow = string.Format("DELETE FROM [Following] WHERE [User_Id] = '{0}' AND [Following_Id] = '{1}'", currentUser, otherUser);
                    string query = followValue ? follow : unfollow;
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.Text;
                        result = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        public List<TweetViewModel> GetAllTweetsByUser(string userId)
        {

            List<TweetViewModel> tweetsList = new List<TweetViewModel>();
            using (SqlConnection con = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("GetAllTweetsByUser", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@User_Id", userId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var model = new TweetViewModel();
                        model.Tweet_Id = Convert.ToInt32(reader["Tweet_Id"]);
                        model.User_Id = reader["User_Id"].ToString();
                        model.UserWithAlias = string.Format("@{0}", reader["User_Id"].ToString());
                        model.Message = reader["Message"].ToString();
                        model.Email = reader["Email"].ToString();
                        model.Created = Convert.ToDateTime(reader["Created"]);
                        var todayDate = DateTime.Now;

                        if (todayDate.Date.ToString("d/M/yyyy").Equals(model.Created.ToString("d/M/yyyy")))
                        {
                            model.Timestamp = model.Created.ToString("HH:mm");
                        }
                        else
                        {
                            model.Timestamp = model.Created.ToString("d MMM");
                        }
                        tweetsList.Add(model);
                    }
                    con.Close();
                }
            }
            return tweetsList;
        }
        public string GetFollowersCount(string userId)
        {
            int result;

            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    string query = string.Format("SELECT Count(1) AS FollowersCount FROM [Following] WHERE [User_Id] = '{0}'", userId);
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.Text;
                        result = (int)cmd.ExecuteScalar();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result.ToString();
        }
        public string GetFollowingCount(string userId)
        {
            int result;

            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    string query = string.Format("SELECT Count(1) AS FollowingCount FROM [Following] WHERE [Following_Id] = '{0}'", userId);
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.Text;
                        result = (int)cmd.ExecuteScalar();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result.ToString();
        }
        public int DeleteTwitterAccount(string userId)
        {
            int result = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand("DeleteTwitterAccount", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        result = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }
    }
}