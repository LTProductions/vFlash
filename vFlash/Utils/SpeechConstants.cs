using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vFlash.Utils
{
    public static class SpeechConstants
    {

        // Speech List<string> assets.
        public static List<string> speechCorrectResponses;
        public static List<string> speechIncorrectResponses;
        public static List<string> speechMotivationResponses;
        public static List<string> speechNextCardResponses;
        public static List<string> speechYesVersions;

        // Load the data.
        public static void LoadData()
        {
            // Initialize speechCorrectResponses.
            speechCorrectResponses = new List<String>();

            // Create and add speechCorrectResponses to the list.
            var correctResponse = "Right! Good job.";
            var correctResponse2 = "Correct! Good job.";
            var correctResponse3 = "Good job!";
            var correctResponse4 = "You're right!";
            var correctResponse5 = "You got it!";
            var correctResponse6 = "Correct!";
            var correctResponse7 = "Right!";
            var correctResponse8 = "Good.";
            var correctResponse9 = "Alright, good job.";
            var correctResponse10 = "Yep, good job!";

            speechCorrectResponses.Add(correctResponse);
            speechCorrectResponses.Add(correctResponse2);
            speechCorrectResponses.Add(correctResponse3);
            speechCorrectResponses.Add(correctResponse4);
            speechCorrectResponses.Add(correctResponse5);
            speechCorrectResponses.Add(correctResponse6);
            speechCorrectResponses.Add(correctResponse7);
            speechCorrectResponses.Add(correctResponse8);
            speechCorrectResponses.Add(correctResponse9);
            speechCorrectResponses.Add(correctResponse10);

            // Initialize speechIncorrectResponses.
            speechIncorrectResponses = new List<String>();

            // Create and add speechIncorrectResponses to the list.
            var incorrectResponse = "I'm sorry, that's wrong.";
            var incorrectResponse2 = "Sorry, that's wrong.";
            var incorrectResponse3 = "I'm sorry, that's incorrect.";
            var incorrectResponse4 = "Sorry, that's incorrect.";
            var incorrectResponse5 = "That's incorrect.";
            var incorrectResponse6 = "Good try, but that's wrong.";
            var incorrectResponse7 = "Nice try, but that's wrong.";
            var incorrectResponse8 = "Good try, but that's incorrect.";
            var incorrectResponse9 = "Nice try, but that's incorrect.";
            var incorrectResponse10 = "I'm sorry, that's not right.";

            speechIncorrectResponses.Add(incorrectResponse);
            speechIncorrectResponses.Add(incorrectResponse2);
            speechIncorrectResponses.Add(incorrectResponse3);
            speechIncorrectResponses.Add(incorrectResponse4);
            speechIncorrectResponses.Add(incorrectResponse5);
            speechIncorrectResponses.Add(incorrectResponse6);
            speechIncorrectResponses.Add(incorrectResponse7);
            speechIncorrectResponses.Add(incorrectResponse8);
            speechIncorrectResponses.Add(incorrectResponse9);
            speechIncorrectResponses.Add(incorrectResponse10);

            // Initialize speechMotivationResponses.
            speechMotivationResponses = new List<String>();

            // Create and add speechMotivationResponses to the list.
            var motivationResponse = "You're doing great!";
            var motivationResponse2 = "We don't have many left to go!";
            var motivationResponse3 = "You're going to master these in no time.";
            var motivationResponse4 = "That test isn't going to know what hit it!";
            var motivationResponse5 = "Wow, someone's been studying.";
            var motivationResponse6 = "Your instructor is going to be impressed.";
            var motivationResponse7 = "Hey, you're pretty good at this.";
            var motivationResponse8 = "Wow, you're nailing these.";
            var motivationResponse9 = "You're awesome at this.";
            var motivationResponse10 = "Wow, you're good.";

            speechMotivationResponses.Add(motivationResponse);
            speechMotivationResponses.Add(motivationResponse2);
            speechMotivationResponses.Add(motivationResponse3);
            speechMotivationResponses.Add(motivationResponse4);
            speechMotivationResponses.Add(motivationResponse5);
            speechMotivationResponses.Add(motivationResponse6);
            speechMotivationResponses.Add(motivationResponse7);
            speechMotivationResponses.Add(motivationResponse8);
            speechMotivationResponses.Add(motivationResponse9);
            speechMotivationResponses.Add(motivationResponse10);


            // Initialize speechNextCardResponses
            speechNextCardResponses = new List<String>();

            // Create and add speechNextCardResponses to the list.
            var nextCard = "Okay, onto the next one.";
            var nextCard2 = "Alright, onto the next one";
            var nextCard3 = "Onto the next one";
            var nextCard4 = "Next card";
            var nextCard5 = "Alright, moving on.";
            var nextCard6 = "Okay, moving on.";
            var nextCard7 = "Moving on";
            var nextCard8 = "Alright, let's keep going.";
            var nextCard9 = "Okay, let's keep going.";
            var nextCard10 = "Let's keep going.";

            speechNextCardResponses.Add(nextCard);
            speechNextCardResponses.Add(nextCard2);
            speechNextCardResponses.Add(nextCard3);
            speechNextCardResponses.Add(nextCard4);
            speechNextCardResponses.Add(nextCard5);
            speechNextCardResponses.Add(nextCard6);
            speechNextCardResponses.Add(nextCard7);
            speechNextCardResponses.Add(nextCard8);
            speechNextCardResponses.Add(nextCard9);
            speechNextCardResponses.Add(nextCard10);


            // Initialize speechYesVersions
            speechYesVersions = new List<String>();

            var yesCard = "yes";
            var yesCard2 = "yeah";
            var yesCard3 = "yup";
            var yesCard4 = "yep";
            var yesCard5 = "sure";
            var yesCard6 = "share";
            var yesCard7 = "shore";
            var yesCard8 = "sore";
            var yesCard9 = "show";
            var yesCard10 = "of course";
            var yesCard11 = "ok";
            var yesCard12 = "okay";
            var yesCard13 = "i guess";
            var yesCard14 = "i guess so";
            var yesCard15 = "guess so";
            var yesCard16 = "yeah i guess";
            var yesCard17 = "ya i guess";
            var yesCard18 = "yeah i guess so";
            var yesCard19 = "ya i guess so";


            speechYesVersions.Add(yesCard);
            speechYesVersions.Add(yesCard2);
            speechYesVersions.Add(yesCard3);
            speechYesVersions.Add(yesCard4);
            speechYesVersions.Add(yesCard5);
            speechYesVersions.Add(yesCard6);
            speechYesVersions.Add(yesCard7);
            speechYesVersions.Add(yesCard8);
            speechYesVersions.Add(yesCard9);
            speechYesVersions.Add(yesCard10);
            speechYesVersions.Add(yesCard11);
            speechYesVersions.Add(yesCard12);
            speechYesVersions.Add(yesCard13);
            speechYesVersions.Add(yesCard14);
            speechYesVersions.Add(yesCard15);
            speechYesVersions.Add(yesCard16);
            speechYesVersions.Add(yesCard17);
            speechYesVersions.Add(yesCard18);
            speechYesVersions.Add(yesCard19);


        }

        /// <summary>
        /// Check all variations of "yes" said by the user. If any match with what the user said, return true.
        /// </summary>
        /// <param name="userResponse"></param>
        /// <returns></returns>
        public static bool CheckForYes(string userResponse)
        {
            foreach (var item in speechYesVersions)
            {
                if (string.Equals(item, userResponse))
                    return true;
            }
            return false;
        }
    }
}
