using SATD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Web.Mvc;

namespace SATD.Controllers
{

    public class HomeController : Controller
    {
       private SATDEntities DBentities = new SATDEntities();
   
        private void FillDropListForParticipent()
        {
            ViewBag.EduLevel = new SelectList(DBentities.Lookups.Where(a => a.LookUpType == 2), "ID", "Description");
            ViewBag.Experience = new SelectList(DBentities.Lookups.Where(a => a.LookUpType == 3), "ID", "Description");
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult participantInfo()
        {
            FillDropListForParticipent();
            return View();
        }

        public ActionResult SaveparticipantInfo(participant participant)
        {
            FillDropListForParticipent();

            if (ModelState.IsValid)
            {
                var CheckEmailExist = (from i in DBentities.participant
                                      where i.Email == participant.Email
                                      select i).FirstOrDefault();
                if (CheckEmailExist!= null)
                {
                    Session["userID"] = CheckEmailExist.ID;
                    return RedirectToAction("classification", "Home", new { @skip = 1 });
                }
                else
                {
                    participant newparticipant = new participant();
                    newparticipant.Email = participant.Email;
                    newparticipant.EduLevel = participant.EduLevel;
                    newparticipant.Experience = participant.Experience;
                    DBentities.participant.Add(newparticipant);
                    DBentities.SaveChanges();
                    Session["userID"] = newparticipant.ID;
                   
                    return RedirectToAction ("classification","Home",new { @skip=1 });

                }
            }
            return View("participantInfo", participant);
        }
        
        public ActionResult classification(Classification classification)
        {
            ViewBag.ClassificationID = new SelectList(DBentities.Lookups.Where(m => m.LookUpType == 1), "ID", "Description");
            if (Session["userID"] == null)
            {
                FillDropListForParticipent();
                return View("participantInfo");
            }
            else
            {
                if (Request.QueryString["skip"]=="1" )
                {
                    GetValidComment();
                }
                else
                {
                    if(ModelState.IsValid)
                    {


                        DBentities.Classification.Add(classification);
                        DBentities.SaveChanges();
                        GetValidComment();
                        ModelState.Clear();

                    }

                    else
                    {
                        int Participentid = Int32.Parse(Session["userID"].ToString());
                        var GetComment = (from i in DBentities.Comments
                                          where i.ID == classification.CommentsID
                                          select i).FirstOrDefault();
                        if (GetComment != null)
                        {
                            ViewBag.Desc = GetComment.CommentsText;
                            ViewBag.CommentsID = GetComment.ID;
                            ViewBag.ParticipantID = Participentid;
                        }
                        else
                        {
                            GetValidComment();
                        }
                    }

                    
                }

                return View();
            }
           
        }
        private void GetValidComment()
        {
            int Participentid = Int32.Parse(Session["userID"].ToString());
            Random rand = new Random();
            int NextComments =  rand.Next(1, DBentities.Comments.Count());
            int flag = 1;
            int Counter = 0;
            //The participent classified this comments befor
            while (flag == 1 && Counter<= DBentities.Comments.Count())
            {
                var checkParticipent = (from j in DBentities.Classification
                                        where j.ParticipantID == Participentid && j.CommentsID == NextComments
                                        select j).FirstOrDefault();
                if (checkParticipent == null)
                {
                    flag = 0;
                    
                }
                else
                {
                    NextComments++;
                }
                Counter++;
            }
            flag = 1;
            Counter = 0;
            // Max number of classification 4 for ecah comments
            while (flag == 1 && Counter <= DBentities.Comments.Count())
            {
                var checkNumberOfCommentsClassified = (from j in DBentities.Classification
                                                       where j.CommentsID == NextComments
                                                       select j).Count();
                if (checkNumberOfCommentsClassified <=4)
                {
                    flag = 0;

                }
                else
                {
                    NextComments++ ;
                }
                Counter++;
            }
            // the number of comments the the participent allow to classify 
            if (DBentities.Classification.Where(a => a.ParticipantID == Participentid).Count() != DBentities.Comments.Count())
            {
                var GetComment = (from i in DBentities.Comments
                                  where i.ID == NextComments
                                  select i).FirstOrDefault();
                if (GetComment != null)
                {
                    ViewBag.Desc = GetComment.CommentsText;
                    ViewBag.CommentsID = GetComment.ID;
                    ViewBag.ParticipantID = Participentid;
                }
            }
            else
            {
                ViewBag.message = "You classify all comments and commits. Thank you";
            }
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "For any question, feel free to contact me anytime.";

            return View();
        }
    }
}