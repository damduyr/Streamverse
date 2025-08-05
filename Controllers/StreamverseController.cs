using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Streamverse.Models;
using System.Web.Security;
namespace Streamverse.Controllers
{
    public class StreamverseController : Controller
    {
        public ApplicationDbConnection db = new ApplicationDbConnection();
        // GET: Streamverse
        [AllowAnonymous]
        public ActionResult DefaultPage()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult RegisterPage()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult RegisterPage(User user)
        {
            if (ModelState.IsValid && user!=null)
            {
                try
                { 
                    db.USERS.Add(user);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message.ToString();
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to Register.Please try again.";
                return View();
            }
            return RedirectToAction("LoginPage");
        }
        [AllowAnonymous]
        public ActionResult LoginPage()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult LoginPage(UserLogin userLoginDetails)
        {
            try
            {
                var admin = db.ADMIN.FirstOrDefault(a => a.Email == userLoginDetails.Email && a.Password == userLoginDetails.Password);
                if (admin != null && admin.activeStatus!=false)
                {
                    Session["UserSession"] = "active";
                    Session["UserType"] = "admin";
                    FormsAuthentication.SetAuthCookie(admin.AdminID.ToString(), false);
                    TempData["adminName"] = admin.First_Name + " " + admin.Last_Name;
                    TempData["LoggedInAdminID"] = admin.AdminID;
                    TempData.Keep();
                    var adminType = admin.AccessLevel;
                    if (adminType == "SystemAdmin")
                    {
                       
                        return RedirectToAction("SystemAdminPage",new {adminID = admin.AdminID });
                    }
                    else if (adminType == "ContentManager")
                    {
                        return RedirectToAction("ContentManagerPage", new { contentManagerID = admin.AdminID });
                    }
                    else
                    {
                        return RedirectToAction("DataAnalystPage", new { dataAnalystID = admin.AdminID });
                    }
                }
                var user = db.USERS.FirstOrDefault(u => u.Email == userLoginDetails.Email && u.Password == userLoginDetails.Password);
                if (user != null)
                {
                    Session["UserSession"] = "active";
                    Session["UserType"] = "user";
                    FormsAuthentication.SetAuthCookie(user.UserID.ToString(), false);
                    var subscription = db.SUBSCRIPTION.FirstOrDefault(s => s.UserID == user.UserID);
                    if (subscription == null)
                    {                       
                        return RedirectToAction("Subscription", new { LoggedInUserID = user.UserID});
                    }
                    else
                    {
                        Session["PlanID"] = subscription.PlanID;
                        TempData.Keep();
                        return RedirectToAction("UserProfile", new { LoggedInUserID = user.UserID });
                    }
                }
                else
                {
                    //user does not exist
                    ViewBag.ErrorMessage = "Invalid Email or Password!";
                    return View();
                }
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMsg = ex.Message;
                return View();
            }
        }

        public ActionResult Subscription(int LoggedInUserID)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"]!="user")
            {
                return RedirectToAction("LoginPage");
            }
            var userSubcriptionDetails = db.SUBSCRIPTION.FirstOrDefault(u=>u.UserID == LoggedInUserID);
            TempData["UserID"] = LoggedInUserID;

            if (userSubcriptionDetails != null)
            {
                return RedirectToAction("UserProfile", new { LoggedInUserID = LoggedInUserID });
            }
            var PlanList = db.SUBSCRIPTION_PLAN.ToList();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return View(PlanList);
        }
        [HttpPost]
        public ActionResult Subscription(SubscriptionPlan planSelected)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "user")
            {
                return RedirectToAction("LoginPage");
            }
            try
            {
                DateTime currDate = DateTime.Now;
                var EndDate = currDate.AddMonths(planSelected.PlanDuration);
                UserSubscription UpdateSelectedPlan = new UserSubscription();
                var userIDTemp = Convert.ToInt32(TempData["UserID"]);
                TempData.Keep();
                UpdateSelectedPlan.UserID = userIDTemp;
                UpdateSelectedPlan.PlanID = planSelected.PlanID;
                UpdateSelectedPlan.StartDate = currDate.Date;
                UpdateSelectedPlan.EndDate = EndDate.Date;
                Session["PlanName"] = planSelected.PlanName;
                Session["PlanID"] = planSelected.PlanID;
                db.SUBSCRIPTION.Add(UpdateSelectedPlan);
                db.SaveChanges();

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                return RedirectToAction("UserProfile", new { LoggedInUserID = userIDTemp });
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("Subscription");
        }

        public ActionResult UserProfile(int LoggedInUserID)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "user")
            {
                return RedirectToAction("LoginPage");
            }
            TempData["UserID"] = LoggedInUserID;
            
            TempData.Keep();
            List<UserProfile> userProfiles = db.USER_PROFILE.Where(p => p.UserID == LoggedInUserID).ToList();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return View(userProfiles);
        }
        [HttpPost]
        public ActionResult SaveUserProfile(string profileName, int profileAge)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "user")
            {
                return RedirectToAction("LoginPage");
            }
            try
            {
                //check if same name exists
                int userIDTemp = (int)TempData["UserID"];
                bool nameExists = db.USER_PROFILE.Any(
                    p => p.ProfileName.Equals(profileName, StringComparison.OrdinalIgnoreCase) 
                    && p.UserID == userIDTemp);

                if (nameExists)
                {
                    //logic to send alert and redirect to action UserProfile
                    //return RedirectToAction("UserProfile", new { LoggedInUserID = (int)TempData["UserID"] });
                    return Json(new { success = false, message="This profile name already exists. Please choose another name."});
                }

                UserProfile profileTemp = new UserProfile();
                profileTemp.UserID = (int)TempData["UserID"];
                profileTemp.ProfileName = profileName;
                profileTemp.Age = profileAge;
                if (profileAge < 18)
                {
                    profileTemp.IsKidProfile = true;
                }
                else
                {
                    profileTemp.IsKidProfile = false;
                }

                db.USER_PROFILE.Add(profileTemp);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("UserProfile", new { LoggedInUserID = (int)TempData["UserID"]});
        }
        public ActionResult RemoveProfile(int profileIdToDelete)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "user")
            {
                return RedirectToAction("LoginPage");
            }
            TempData.Keep();
            if (Session["UserSession"] == null)
            {
                return RedirectToAction("LoginPage");
            }

            try
            {
                var profile = db.USER_PROFILE.Find(profileIdToDelete);
                if(profile != null)
                {
                    var WatchlistItems = db.WATCHLIST.Where(w => w.ProfileID == profileIdToDelete);
                    db.WATCHLIST.RemoveRange(WatchlistItems);

                    var reviews = db.REVIEW.Where(r=>r.ProfileID == profileIdToDelete);
                    db.REVIEW.RemoveRange(reviews);

                    db.USER_PROFILE.Remove(profile);

                    db.SaveChanges();
                    
                }
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("UserProfile", new { LoggedInUserID = (int)TempData["UserID"] });
        }
        public ActionResult HomePage(int LoggedInUserID, int profileID)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "user")
            {
                return RedirectToAction("LoginPage");
            }
            var PlanID = (int)Session["PlanID"];
            var userPlan = db.SUBSCRIPTION_PLAN.Find(PlanID);
            Session["PlanName"] = userPlan.PlanName;

            var userProfile = db.USER_PROFILE.Find(profileID);
            TempData["profileName"] = userProfile.ProfileName;
            TempData["ProfileID"] = profileID;
            
            var allMovies = (from content in db.CONTENT
                             join movie in db.MOVIE
                             on content.ContentID equals movie.MovieID
                             join genre in db.GENRE
                             on content.GenreID equals genre.GenreID
                             select new HomePageViewModel.MovieSummmaryViewModel
                             {
                                 MovieID = content.ContentID,
                                 Title = content.Title,
                                 Description = content.Description,
                                 ReleaseDate = content.ReleaseDate,
                                 ContentType = content.ContentType,
                                 GenreName = genre.GenreName,
                                 DurationInMinutes = movie.DurationInMinutes,
                                 PosterURL = movie.PosterURL,
                                 TrailerURL = movie.TrailerURL,
                                 VideoUrl = movie.VideoUrl,
                                 OverallRating = movie.OverallRating
                             }).ToList();
            //
            var filteredMovies = new List<HomePageViewModel.MovieSummmaryViewModel>();
           //Silver Gold Platinum
            //filteredMovies = allMovies.Where(movie => movie.ContentType == userPlan.PlanName).ToList();
            ///
            var accessibleContentTypes = new List<string>();
            if(userPlan.PlanName == "Platinum")
            {
                accessibleContentTypes.AddRange(new[] { "Platinum", "Gold", "Silver" });
            }
            else if (userPlan.PlanName == "Gold")
            {
                accessibleContentTypes.AddRange(new[] {"Gold", "Silver" });
            }
            else if (userPlan.PlanName == "Silver")
            {
                accessibleContentTypes.Add("Silver");
            }
            //filter movies based on plan
            filteredMovies = allMovies.Where(movie => accessibleContentTypes.Contains(movie.ContentType)).ToList();


            //top 5 Trending movies list
            var sevenYearAgo = DateTime.Now.AddYears(-7);

            var top5TrendingMoviesList = filteredMovies.Where(
                movie => movie.ReleaseDate >= sevenYearAgo
                ).OrderByDescending(movie => movie.OverallRating).Take(5).ToList();

            //Popular movies in region
            var userRegion = db.USERS
                .Where(u => u.UserID == LoggedInUserID)
                .Select(u => u.RegionName).FirstOrDefault();
            TempData["userRegion"] = userRegion;
            TempData.Keep();

            //var topRegionalMovies = (from watchlist in db.WATCHLIST
            //                            join profile in db.USER_PROFILE on watchlist.ProfileID equals profile.ProfileID
            //                            join user in db.USERS on profile.ProfileID equals user.UserID
            //                            where user.RegionName == userRegion
            //                            group watchlist by watchlist.ContentID into movieGroup
            //                            orderby movieGroup.Count() descending
            //                            select new
            //                            {
            //                                ContentID = movieGroup.Key,                                            
            //                            }).Take(5)
            //                       .Join(db.CONTENT,
            //                       popularMovie => popularMovie.ContentID,
            //                       content => content.ContentID,
            //                       (popularMovie, content) => new { content })
            //                       .Join(db.MOVIE,
            //                       result => result.content.ContentID,
            //                       movie => movie.MovieID,
            //                       (result, movie) => new HomePageViewModel.MovieSummmaryViewModel
            //                       {
            //                           MovieID = movie.MovieID,
            //                           Title = result.content.Title,
            //                           Description = result.content.Description,
            //                           ReleaseDate = result.content.ReleaseDate,
            //                           ContentType = result.content.ContentType,
            //                           DurationInMinutes = movie.DurationInMinutes,
            //                           TrailerURL = movie.TrailerURL,
            //                           VideoUrl = movie.VideoUrl,
            //                           PosterURL = movie.PosterURL,
            //                           OverallRating = movie.OverallRating,
            //                           GenreName = "Action"
            //                       }).ToList();

            var topRegionalMovies = (from user in db.USERS
                                     join profile in db.USER_PROFILE
                                     on user.UserID equals profile.UserID
                                     join watchlist in db.WATCHLIST
                                     on profile.ProfileID equals watchlist.ProfileID
                                     where user.RegionName == userRegion
                                     group watchlist by watchlist.ContentID
                                     into movieGroup orderby movieGroup.Count() descending
                                     select new
                                     {
                                         ContentID = movieGroup.Key,
                                     }).Take(5)
                                    .Join(db.CONTENT,
                                        popularMovie => popularMovie.ContentID,
                                        content => content.ContentID,
                                        (popularMovie, content) => new { content })
                                    .Join(db.MOVIE,
                                        result => result.content.ContentID,
                                        movie => movie.MovieID,
                                        (result, movie) => new HomePageViewModel.MovieSummmaryViewModel
                                        {
                                            MovieID = movie.MovieID,
                                            Title = result.content.Title,
                                            Description = result.content.Description,
                                            ReleaseDate = result.content.ReleaseDate,
                                            ContentType = result.content.ContentType,
                                            DurationInMinutes = movie.DurationInMinutes,
                                            TrailerURL = movie.TrailerURL,
                                            VideoUrl = movie.VideoUrl,
                                            PosterURL = movie.PosterURL,
                                            OverallRating = movie.OverallRating,
                                            GenreName = " "
                                        }).ToList();

            //End of task 3
            // Watchlist
            var watchlistData = db.WATCHLIST.Where(w => w.ProfileID == profileID).Select(w => new { w.ContentID, w.DateAdded }).ToList();
            var watchlistContentIDs = watchlistData.Select(w => w.ContentID).ToList();
            var unsortedWatchlistMovies = allMovies.Where(m => watchlistContentIDs.Contains(m.MovieID)).ToList();
            var sortedWatchlistMovies = (from movie in unsortedWatchlistMovies
                                         join data in watchlistData on movie.MovieID equals data.ContentID
                                         orderby data.DateAdded descending
                                         select movie).ToList();
            //Top 5 movies from watchlist depending on date added
            var top5WatchlistMovies = sortedWatchlistMovies.Take(5).ToList();

          
            var MovieViewModel = new HomePageViewModel
            {
                Top5TrendingMovies = top5TrendingMoviesList,
                AllMovies = allMovies,
                FilteredMovies = filteredMovies,
                TopRegionalMovies = topRegionalMovies,
                SortedWatchlistMovies = sortedWatchlistMovies,
                Top5WatchlistMovies = top5WatchlistMovies
            };

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return View(MovieViewModel);
        }
        public ActionResult AddToWatchlist(int profileID, int contentID)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "user")
            {
                return RedirectToAction("LoginPage");
            }
            try
            {
                if (db.WATCHLIST.Any(w => w.ProfileID == profileID && w.ContentID == contentID))
                {
                    TempData["WatchlistNotification"] = "Movie already present in the watchlist.";
                }
                else
                {
                    Watchlist temp_list = new Watchlist();
                    temp_list.ContentID = contentID;
                    temp_list.DateAdded = DateTime.Now;
                    temp_list.ProfileID = profileID;

                    db.WATCHLIST.Add(temp_list);
                    db.SaveChanges();
                    TempData["WatchlistNotification"] = "Added to Watchlist Successfully.";
                }
            }
            catch(Exception ex)
            {
                TempData["WatchlistNotification"] = ex.Message;
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("HomePage", new { LoggedInUserID = (int)TempData["UserID"], profileID = profileID });
        }
        public ActionResult RemoveFromWatchlist(int profileID, int contentID)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "user")
            {
                return RedirectToAction("LoginPage");
            }
            try
            {
                TempData.Keep();
                var selectedMovieToRemove = db.WATCHLIST.Where(w => w.ProfileID == profileID && w.ContentID == contentID).FirstOrDefault();
                if (selectedMovieToRemove == null)
                {
                    TempData["WatchlistNotification"] = "Error: Movie doesn't exists in your Watchlist.";
                }
                else
                {
                    db.WATCHLIST.Remove(selectedMovieToRemove);
                    db.SaveChanges();
                    TempData["WatchlistNotification"] = "Successfully removed from Watchlist.";
                }
            }
            catch (Exception ex)
            {
                TempData["WatchlistNotification"] = ex.Message;
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("HomePage", new { LoggedInUserID = (int)TempData["UserID"], profileID = profileID });
        }
        [HttpPost]
        public ActionResult SaveReview(int profileID, int movieIDTemp, int ratingValue, string ratingText)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "user")
            {
                return RedirectToAction("LoginPage");
            }
            
            //if(ratingValue <1 || ratingValue > 5)
            //{
            //    TempData["WatchlistNotification"] = "Enter rating value between 1 and 5.";
            //    return RedirectToAction("HomePage", new { LoggedInUserID = (int)TempData["UserID"], profileID = profileID });
            //}

            Review review = new Review();
            review.ProfileID = profileID;
            review.ContentID = movieIDTemp;
            review.RatingValue = ratingValue;
            review.RatingText = ratingText;
            db.REVIEW.Add(review);
            db.SaveChanges();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("HomePage", new { LoggedInUserID = (int)TempData["UserID"], profileID = profileID });
        }

        //ADMIN METHODS
        public ActionResult SystemAdminPage(int adminID)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "admin")
            {
                return RedirectToAction("LoginPage");
            }
            TempData["LoggedInAdminID"] = adminID;
            TempData.Keep();
            var adminList = db.ADMIN.ToList();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return View(adminList);
        }
        public ActionResult ShowUsersList()
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "admin")
            {
                return RedirectToAction("LoginPage");
            }
            var userList = db.USERS.ToList();
            try
            {
                if (userList == null)
                {
                    return HttpNotFound();
                }
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return View(userList);
        }
        public ActionResult EditAdminDetails(int selectedAdminID)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "admin")
            {
                return RedirectToAction("LoginPage");
            }
            TempData.Keep();
            //Student Obj_temp = StudentList.myStudentList.Where(m => m.id == id).FirstOrDefault();
            Admin record = db.ADMIN.Find(selectedAdminID);
            if (record == null)
            {
                return HttpNotFound();
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return View(record);
        }
        [HttpPost]
        public ActionResult EditAdminDetails(Admin editedAdmin)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "admin")
            {
                return RedirectToAction("LoginPage");
            }
            TempData.Keep();
            Admin record = db.ADMIN.Find(editedAdmin.AdminID);
            if (record == null)
            {
                return HttpNotFound();
            }
            else
            { 
                record.First_Name = editedAdmin.First_Name;
                record.Last_Name = editedAdmin.Last_Name;
                record.Password = editedAdmin.Password;
                record.Username = editedAdmin.Username;
                record.AccessLevel = editedAdmin.AccessLevel;
                record.activeStatus = editedAdmin.activeStatus;
                db.SaveChanges();
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("SystemAdminPage", new { adminID = (int)TempData["LoggedInAdminID"] });
        }
        public ActionResult DeleteAdminDetails(int selectedAdminID)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "admin")
            {
                return RedirectToAction("LoginPage");
            }
            TempData.Keep();
            Admin record = db.ADMIN.Find(selectedAdminID);
            if (record == null)
            {
                return HttpNotFound();
            }
            record.activeStatus = false;
            db.SaveChanges();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("SystemAdminPage", new { adminID = (int)TempData["LoggedInAdminID"] });
        }
        public ActionResult AddNewAdmin()
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "admin")
            {
                return RedirectToAction("LoginPage");
            }
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return View();
        }
        [HttpPost]
        public ActionResult AddNewAdmin(Admin newAdmin)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "admin")
            {
                return RedirectToAction("LoginPage");
            }
            try
            {
                TempData.Keep();
                db.ADMIN.Add(newAdmin);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("SystemAdminPage", new { adminID = (int)TempData["LoggedInAdminID"] });
        }
        public ActionResult ContentManagerPage(int contentManagerID)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "admin")
            {
                return RedirectToAction("LoginPage");
            }
            TempData["LoggedInAdminID"] = contentManagerID;
            TempData["contentManagerID"] = contentManagerID;
            TempData.Keep();
            var allMovies = (from content in db.CONTENT
                                                 join movie in db.MOVIE
                                                 on content.ContentID equals movie.MovieID
                                                 join genre in db.GENRE
                                                 on content.GenreID equals genre.GenreID
                                                 select new HomePageViewModel.MovieSummmaryViewModel
                                                 {
                                                     MovieID = content.ContentID,
                                                     Title = content.Title,
                                                     Description = content.Description,
                                                     ReleaseDate = content.ReleaseDate,
                                                     ContentType = content.ContentType,
                                                     GenreName = genre.GenreName,
                                                     DurationInMinutes = movie.DurationInMinutes,
                                                     PosterURL = movie.PosterURL,
                                                     TrailerURL = movie.TrailerURL,
                                                     VideoUrl = movie.VideoUrl,
                                                     OverallRating = movie.OverallRating
                                                 }).ToList();

            var MovieViewModel = new HomePageViewModel
            {
                AllMovies = allMovies
            };

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return View(MovieViewModel);
        }
        public ActionResult EditMovieDetails(int movieID)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "admin")
            {
                return RedirectToAction("LoginPage");
            }

            var fullMovieDetails = (from content in db.CONTENT
             join movie in db.MOVIE
             on content.ContentID equals movie.MovieID
             join genre in db.GENRE
             on content.GenreID equals genre.GenreID
                                    where content.ContentID == movieID
             select new FullMovieDetails
             {
                 MovieID = content.ContentID,
                 Title = content.Title,
                 Description = content.Description,
                 ReleaseDate = content.ReleaseDate,
                 ContentType = content.ContentType,
                 GenreName = genre.GenreName,
                 DurationInMinutes = movie.DurationInMinutes,
                 PosterURL = movie.PosterURL,
                 TrailerURL = movie.TrailerURL,
                 VideoUrl = movie.VideoUrl,
                 OverallRating = movie.OverallRating
             }).ToList().FirstOrDefault();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return View(fullMovieDetails);
        }
        [HttpPost]
        public ActionResult EditMovieDetails(FullMovieDetails editedMovieDetails)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "admin")
            {
                return RedirectToAction("LoginPage");
            }
            try
            {
                var genreID = db.GENRE.Where(g => g.GenreName == editedMovieDetails.GenreName).Select(g => g.GenreID).FirstOrDefault();

                var temp_content = db.CONTENT.Find(editedMovieDetails.MovieID);
                temp_content.AdminID = (int)TempData["LoggedInAdminID"];
                TempData.Keep();
                temp_content.Title = editedMovieDetails.Title;
                temp_content.Description = editedMovieDetails.Description;
                temp_content.ContentType = editedMovieDetails.ContentType;
                temp_content.GenreID = genreID;

                var temp_movie = db.MOVIE.Find(editedMovieDetails.MovieID);
                temp_movie.DurationInMinutes = editedMovieDetails.DurationInMinutes;
                temp_movie.TrailerURL = editedMovieDetails.TrailerURL;
                temp_movie.PosterURL = editedMovieDetails.PosterURL;
                temp_movie.VideoUrl = editedMovieDetails.VideoUrl;
                temp_movie.OverallRating = editedMovieDetails.OverallRating;
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("ContentManagerPage", new { contentManagerID = (int)TempData["LoggedInAdminID"] });
        }
        public ActionResult DeleteMovieDetails(int movieID)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "admin")
            {
                return RedirectToAction("LoginPage");
            }
            try
            {
                var movie_record = db.MOVIE.Find(movieID);
                if (movie_record == null) return HttpNotFound();
                else
                {
                    var watchlist_record = db.WATCHLIST.Where(w => w.ContentID == movieID).FirstOrDefault(); ;
                    if(watchlist_record!=null) db.WATCHLIST.Remove(watchlist_record);
                    var review_record = db.REVIEW.Where(r => r.ContentID == movieID).FirstOrDefault();
                    if (review_record != null) db.REVIEW.Remove(review_record);
                    var content_record = db.CONTENT.Find(movieID);
                    if (content_record == null) return HttpNotFound();
                    db.MOVIE.Remove(movie_record);
                    db.CONTENT.Remove(content_record);
                    db.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("ContentManagerPage", new { contentManagerID = (int)TempData["contentManagerID"] });
        }
        public ActionResult AddMovie()
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "admin")
            {
                return RedirectToAction("LoginPage");
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return View();
        }
        [HttpPost]
        public ActionResult AddMovie(FullMovieDetails newMovieDetails)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "admin")
            {
                return RedirectToAction("LoginPage");
            }
            try
            {
                var genreID = db.GENRE.Where(g => g.GenreName == newMovieDetails.GenreName).Select(g => g.GenreID).FirstOrDefault();

                Content temp_content = new Content();
                temp_content.ContentID = newMovieDetails.MovieID;
                temp_content.AdminID = (int)TempData["LoggedInAdminID"];
                TempData.Keep();
                temp_content.Title = newMovieDetails.Title;
                temp_content.Description = newMovieDetails.Description;
                temp_content.ReleaseDate = newMovieDetails.ReleaseDate;
                temp_content.ContentType = newMovieDetails.ContentType;
                temp_content.GenreID = genreID;
                db.CONTENT.Add(temp_content);

                Movie temp_movie = new Movie();
                temp_movie.MovieID = newMovieDetails.MovieID;
                temp_movie.DurationInMinutes = newMovieDetails.DurationInMinutes;
                temp_movie.TrailerURL = newMovieDetails.TrailerURL;
                temp_movie.PosterURL = newMovieDetails.PosterURL;
                temp_movie.VideoUrl = newMovieDetails.VideoUrl;
                temp_movie.OverallRating = newMovieDetails.OverallRating;
                db.MOVIE.Add(temp_movie);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("ContentManagerPage", new { contentManagerID = (int)TempData["LoggedInAdminID"] });
        }

        public ActionResult DataAnalystPage(int dataAnalystID)
        {
            TempData.Keep();
            if (Session["UserSession"] == null || (string)Session["UserType"] != "admin")
            {
                return RedirectToAction("LoginPage");
            }
            TempData["LoggedInAdminID"] = dataAnalystID;
            TempData["dataAnalystID"] = dataAnalystID;
            TempData.Keep();

            List<MoviePopularityViewModel> moviePopularity = db.WATCHLIST.GroupBy(item => item.Content.Title)
                .Select(group => new
                {
                    ContentTitle = group.Key,
                    Adds = group.Count()
                }).ToList().Select(result => new MoviePopularityViewModel
                {
                    MovieTitle = result.ContentTitle,
                    WatchlistCount = result.Adds
                }).OrderByDescending(p => p.WatchlistCount).ToList();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return View(moviePopularity);
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            TempData.Clear();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("LoginPage");
        }
    }
}