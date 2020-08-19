using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using System.Configuration;
using Selenium.Scripts.Pages;

namespace Selenium.Scripts.Reusable.Generic
{
    class TestCaseResult
    {
        public String status;
        public TestStep[] steps;

        public TestCaseResult(int stepcount)
        {
            steps = new TestStep[stepcount];
            for (int i = 0; i < stepcount; i++)
            {
                this.steps[i] = new TestStep();
                this.steps[i].stepnumber = i + 1;

            }
        }

        /// <This function will finalize the overall Test result; based on the Test steps result>
        /// 
        /// </summary>
        /// <returns></returns>
        public String FinalResult()
        {  
            String Status = "Pass";

           for (int i =0; i<steps.Length; i++)
           {
               if (this.steps[i].status.ToLower().Contains("fail"))
               {
                   Status = "Fail";
               }
               else if (this.steps[i].status.ToLower().Contains("no run"))
               {
                   this.steps[i].status="Skip";
                   Status = "Fail";
               }
               else 
               {
                  //Test step is passed but Test case status not affected    
               }
           }

           this.status = Status;
           return Status;
        }

        public void FinalResult(int executedsteps)
        {
            this.status = "Pass";
            Boolean allstepsrun = false;
            int passedfailedindex = 0;

            //compare steps executed and Total test steps
            if (this.steps.Length == executedsteps+1)
            {
                allstepsrun = true;
            }

            //This Logic is when all the steps have been executed with some pass and fail
            if (allstepsrun)
            {
                //Make all No Runs to Pass Status
                for (int i = 0; i < steps.Length; i++)
                {

                    if (steps[i].status.ToLower().Equals("no run"))
                    {
                        steps[i].status = "Pass";
                    }
                }
                this.SetFinalStatus();
                return;
            }
               
                //This logic is when not all steps got executed and exception raised either in first step, last step or in middle.
            if (!allstepsrun)
            {
                //Find the first passed or failed Test step
                for (int i = 0; i < steps.Length; i++)
                {
                    if (steps[i].status.ToLower().Equals("pass") || steps[i].status.ToLower().Equals("Fail"))
                    {
                        passedfailedindex = i;
                        break;
                    }
                }

                //If all Test steps are no run status and exception thrown while executing first step
                if (passedfailedindex == 0 && steps[0].status.ToLower().Equals("no run") && executedsteps==-1)
                {
                    
                    //Make First step as failed
                    steps[0].status = "Fail";

                    //Make following status as skipped
                    for (int i = 1; i < this.steps.Length; i++)
                    {
                        if (this.steps[i].status.ToLower().Equals("no run"))
                            this.steps[i].status = "Skip";  
                    }

                    this.SetFinalStatus();
                    return;
                }

                //If first step is failed and exception also is thrown
                if (passedfailedindex == 0 && steps[0].status.ToLower().Equals("fail") && executedsteps==0)
                {
                     //Skip the following steps
                    for (int i = 1; i < steps.Length; i++)
                    {
                        if (this.steps[i].status.ToLower().Equals("no run"))
                            this.steps[i].status = "Skip";
                    }
                    this.SetFinalStatus();
                    return;
                }

                //If first and other steps are passed but exception riased in the middle
                if (passedfailedindex == 0 && steps[0].status.ToLower().Equals("pass") && executedsteps>-1)                    
                 {
                    //Pass till executed steps which are in no run state
                    for (int i =0 ; i<=executedsteps; i++)
                    {
                        if (this.steps[i].status.ToLower().Equals("no run"))
                            this.steps[i].status = "Pass";
                    }

                    //Fail the step next after the executed steps
                    this.steps[executedsteps + 1].status = "Fail";

                    //Skip the others.
                    for (int i = executedsteps+2; i <steps.Length; i++)
                    {
                        if (this.steps[i].status.ToLower().Equals("no run"))
                            this.steps[i].status = "Skip";
                    }
                    this.SetFinalStatus();
                    return;
                }

                //If first and other steps are executed and exception raised in middle
                if (passedfailedindex == 0 && steps[0].status.ToLower().Equals("no run") && executedsteps>-1)
                {
                    //Pass till executed steps which are in no run state
                    for (int i = 0; i <= executedsteps; i++)
                    {
                        if (this.steps[i].status.ToLower().Equals("no run"))
                            this.steps[i].status = "Pass";
                    }

                    //Fail the step next after the executed steps
                    this.steps[executedsteps + 1].status = "Fail";

                    //Skip the others.
                    for (int i = executedsteps + 2; i < steps.Length; i++)
                    {
                        if (this.steps[i].status.ToLower().Equals("no run"))
                            this.steps[i].status = "Skip";
                    }
                    this.SetFinalStatus();
                    return;
                }

                //If first and other steps are executed and exception raised in middle
                if (passedfailedindex == 0 && steps[0].status.ToLower().Equals("fail") && executedsteps > -1)
                {
                    //Pass till executed steps which are in no run state
                    for (int i = 0; i <= executedsteps; i++)
                    {
                        if (this.steps[i].status.ToLower().Equals("no run"))
                            this.steps[i].status = "Pass";
                    }

                    //Fail the step next after the executed steps
                    this.steps[executedsteps + 1].status = "Fail";

                    //Skip the others.
                    for (int i = executedsteps + 2; i < steps.Length; i++)
                    {
                        if (this.steps[i].status.ToLower().Equals("no run"))
                            this.steps[i].status = "Skip";
                    }
                    this.SetFinalStatus();
                    return;
                }


                //If first and other steps are passed but exception riased in the middle
                if (passedfailedindex > 0)
                {
                    //Pass till executed steps which are in no run state
                    for (int i = 0; i <= executedsteps; i++)
                    {
                        if (this.steps[i].status.ToLower().Equals("no run"))
                            this.steps[i].status = "Pass";
                    }

                    //Either Fail or skip the step next after the executed steps
                    if (this.steps[executedsteps].status.ToLower().Equals("fail"))
                    {
                        this.steps[executedsteps + 1].status = "Skip";
                    }
                    else
                    {
                        this.steps[executedsteps + 1].status = "Fail";
                    }

                        //Skip the others stes which are in no run status.
                        for (int i = executedsteps + 2; i < steps.Length; i++)
                        {
                            if (this.steps[i].status.ToLower().Equals("no run"))
                                this.steps[i].status = "Skip";
                        }
                    this.SetFinalStatus();
                    return;
                }
            }                          
            
          }

        public void FinalResult(Exception exception)
        {
            this.FinalResult();

            //Set the comment with exception object
            foreach (TestStep step in this.steps)
            {
                if (step.status.ToLower().Equals("fail"))
                {
                    step.comments = exception.Message + Environment.NewLine + exception.StackTrace;
                }
            }

        }
         
        public void FinalResult(Exception exception, int executedsteps)
        {   
            //Update the status of each steps
            this.FinalResult(executedsteps);
            
            //Set the comment with exception object
            int ignoresteps = 0;
            foreach(TestStep step in this.steps)
            {
                if (executedsteps == -1)
                {
                    step.actualresult = exception.Message;
                    step.comments = exception.Message + Environment.NewLine + exception.StackTrace;
                    break;
                }
                if (ignoresteps <= executedsteps) { ignoresteps++; continue; }
                if (step.status.ToLower().Equals("fail"))
                {
                    step.actualresult = exception.Message;
                    step.comments = exception.Message + Environment.NewLine + exception.StackTrace;
                    break;
                }               
            }

            //Set the snapshot path
            if (executedsteps==-1)
            {
                executedsteps++;
                this.steps[executedsteps].SetLogs();
                return;
            }
            if (String.IsNullOrEmpty(this.steps[executedsteps].snapshotpath))
            {
                if (executedsteps.Equals(this.steps.Length-1)) //If imagecomaprison is done at the final step of a testcase
                {
                    this.steps[executedsteps].SetLogs();
                    this.steps[executedsteps].status = "FAIL";
                    this.SetFinalStatus();
                }
                else { this.steps[executedsteps + 1].SetLogs(); }
            }           
         }

        /// <summary>
        /// This Method will setup the description for the Test steps
        /// </summary>
        /// <param name="descriptions"></param>
        public void SetTestStepDescription(String descriptions)
        {   
            String[] steps_expectedresults = descriptions.Split('=');
            String[] stepdetails = steps_expectedresults[0].Split(':');
            String[] expectedresults = steps_expectedresults[1].Split(':');
            String[] actualresults = steps_expectedresults[2].Split(':');

            //Set the Step Object with test descriptions
            int iterate = 0;
            foreach (TestStep step in this.steps)
            {
                step.description = stepdetails[iterate];
                iterate++;
            }

            //Set the Step Object with expectedreults
            int iterate1 = 0;
            foreach (TestStep step in this.steps)
            {
                step.expectedresult = expectedresults[iterate1];
                iterate1++;
            }

            //Set the Step Object with actualreults
            int iterate2 = 0;
            foreach (TestStep step in this.steps)
            {
                try
                {
                    step.actualresult = actualresults[iterate2];
                }
                catch (Exception exp)
                {
                    step.actualresult = "";
                }
                iterate2++;
            }

        }

        public void SetFinalStatus()
        {
            //Set the Final Status
            foreach (TestStep step in this.steps)
            {
                if (step.status.ToLower().Equals("fail") || step.status.ToLower().Equals("skipped"))
                {
                    this.status = "Fail";
                    if (step.status.ToLower().Equals("fail") && step.actualresult.ToLower().Equals("step working properly as expected"))
                        step.actualresult = "Step not working as expected";
                    else if (step.status.ToLower().Equals("skipped") && step.actualresult.ToLower().Equals("step working properly as expected"))
                        step.actualresult = "";
                    break;
                }
            } 

        }
             
    }
}