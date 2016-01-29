using System.Collections.Generic;
using Loewenbraeu.Model;

namespace Loewenbraeu.Data
{
	public class QuizRepository
	{
		
		public static Quiz GetQuiz ()
		{
			var quiz = new Quiz ();
			quiz.Id = 6;
			quiz.PointsProAnswer = 3;
			quiz.Questions = QuizRepository.GetQuestions ();
			
			return quiz;
		}
		
		
		
		public static List<Question> GetQuestions ()
		{
			var questions = new List<Question> ();
			var answers = new List<Answer> ();
			
			//Question 1 - Frage 8
			answers.Add (new Answer (){ 
				Text="Reichster Mann Bayerns"
			});
			answers.Add (new Answer (){ 
				Text="Stärkster Mann Bayerns", 
				Correct=true,
				Explanation="Der Münchner Metzger galt anno 1879 als 'Stärkster Mann Bayerns'. Denn es ihm gelang einen 259 kg schweren Stein nur mit einem Finger anzulupfen. Die Tradition des Steinhebens findet auch heute noch während der Starkbierzeit im Löwenbräukeller statt."
			});
			answers.Add (new Answer (){ 
				Text="Schönster Mann Bayerns"
			});
			
			questions.Add (new Question (){
				Text="Mit welchem Titel verewigte sich der legendäre Steyrer Hans in den Geschichtsbüchern Bayerns?", 
				Answers = answers, 
				Category = QuestionCategory.BAY
			});
			
			//Question 2 - Frage 1
			answers = new List<Answer> ();
			answers.Add (new Answer (){ 
				Text="Kiss", 
				Correct=true,
				Explanation="Ganz ohne Makeup rockten Kiss hier vor 2000 begeisterten Fans."
			});
			answers.Add (new Answer (){ 
				Text="Quen"
			});
			answers.Add (new Answer (){ 
				Text="The Sweet"
			});
			
			questions.Add (new Question (){
				Text="Welche Band spielte 1983 live im Löwenbräukeller?", 
				Answers = answers,
				Category = QuestionCategory.LBK
			});
			
			//Question 3 - Frage 17
			answers = new List<Answer> ();
			answers.Add (new Answer (){ 
				Text="Im Englischen Garten"
			});
			answers.Add (new Answer (){ 
				Text="Im Wirtshaus & Biergarten", 
				Correct=true,
				Explanation="Das traditionelle Schmalzgebäck ist ein Klassiker der Bayerischen Süßen Küche. Der Hefeteig wird dabei aus der Mitte heraus rund ausgezogen. In anderen Gegenden sind sie auch als Küchl bekannt."
			});
			
			answers.Add (new Answer (){ Text="Am Flaucher"});
			
			questions.Add (new Question (){
				Text="Wo findet man in München meistens immer eine 'Ausgezogene'?", 
				Answers = answers, 
				Category = QuestionCategory.FOD
			});
			
			/*
			//Question 4 - Frage 11
			answers = new List<Answer> ();
			answers.Add (new Answer (){ 
				Text="Siegessäule"
			});
            answers.Add (new Answer ()
            { 
                Text = "Friedensengel"
            });

            answers.Add(new Answer()
            {
                Text = "Mariensäule",
                Correct = true,
                Explanation = "Weil der schwedische König Gustav II. Adolf München im Dreißigjährigen Krieg nicht zerstörte, ließ Kurfürst Maximilian I. 1638 als Dank an Gott die Mariensäule errichten."
            });
			
			questions.Add (new Question (){
				Text = "Welche Säule steht im Mittelpunkt der Bayerischen Landeshauptstadt?", 
				Answers = answers, 
				Category = QuizCategory.MUC
			});

            //Question 5 - Frage 5
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "850 Jahre",
                Correct = true,
                Explanation = "Urkundlich wurde München erstmals 1158 erwähnt. Regiert wurde es vom Welfen-Herzog Heinrich der Löwe."
            });
            answers.Add(new Answer()
            {
                Text = "600 Jahre"
            });
            answers.Add(new Answer()
            {
                Text = "1100 Jahre"
            });

            questions.Add(new Question()
            {
                Text = "Welchen runden Geburtstag feierte München 2008?",
                Answers = answers,
                Category = QuizCategory.MUC
            });

            //Question 6 - Frage 9
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "1901"
            });
            answers.Add(new Answer()
            {
                Text = "1890"
            });
            answers.Add(new Answer()
            {
                Text = "1867",
                Correct = true,
                Explanation = "Genau 50 Personen fanden anno 1867 Platz in dem ersten Biertempel auf dem Oktoberfest. Allerdings handelt es sich dabei noch um kein Zelt, sondern einen Bretterschuppen."    
            });

            questions.Add(new Question()
            {
                Text = "Wann gründete die Familie Schottenhamel das 'erste Wiesn-Zelt' auf der Theresienwiese?",
                Answers = answers,
                Category = QuizCategory.SCH
            });

            //Question 7 - Frage 2
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "29. Februar 1901"
            });
            answers.Add(new Answer()
            {
                Text = "14. Juni 1883",
                Correct = true,
                Explanation = "Insgesamt 413.311.11 Mark zahlte der Brauer und Löwenbräu-Eigentümer Ludwig Brey für den Bau."
            });
            answers.Add(new Answer()
            {
                Text = "3. November 1814"
            });

            questions.Add(new Question()
            {
                Text = "Wann wurde der Löwenbräukeller feierlich eröffnet?",
                Answers = answers,
                Category = QuizCategory.LBK
            });
			
            //Question 8 - Frage 3
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "Bier-Pipeline"
            });
            answers.Add(new Answer()
            {
                Text = "Keferloher-Spülmaschine"
            });
            answers.Add(new Answer()
            {
                Text = "Elektrisches Licht",
                Correct = true,
                Explanation = "Das gesamte Gebäude, sowohl außen als auch innen, verfügte über eine elektrische Beleuchtung."
            });

            questions.Add(new Question()
            {
                Text = "Welche Sensation bot der Löwenbräukeller um 1894 den Münchnern?",
                Answers = answers,
                Category = QuizCategory.LBK
            });

            //Question 9 - Frage 4
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "Großbrand",
                Correct = true,
                Explanation = "Über Nacht brannte der gerade erst vollkommen renovierte Festsaal samt Galerie und Balkon ab."
            });
            answers.Add(new Answer()
            {
                Text = "Guiness-Hochzeitsrekord"
            });
            answers.Add(new Answer()
            {
                Text = "Wiedereröffnung"
            });

            questions.Add(new Question()
            {
                Text = "Was geschah am 23. Juli 1986 im Löwenbräukeller?",
                Answers = answers,
                Category = QuizCategory.LBK
            });

            //Question 10 - Frage 6
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "anno 1480"
            });
            answers.Add(new Answer()
            {
                Text = "anno 1553"
            });
            answers.Add(new Answer()
            {
                Text = "anno 1383",
                Correct = true,
                Explanation = "Bis 1383 lässt sich die Bräu-Geschichte zurückverfolgen. 1818 erwirbt der Brauer Georg Brey die Löwenbrauerei und macht Löwenbräu zur größten Brauerei."
            });

            questions.Add(new Question()
            {
                Text = "Wann wurde die Löwenbräu-Brauerei gegründet?",
                Answers = answers,
                Category = QuizCategory.BIE
            });

            //Question 11 - Frage 7
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "nach Aschermittwoch",
                Correct = true,
                Explanation = "Nach dem Ende des Faschings beginnt die Zeit des Fasten und somit auch die Starkbierzeit. Im Löwenbräukeller wird dann vier Wochen lang ein süffiges Triumphator ausgeschenkt."
            });
            answers.Add(new Answer()
            {
                Text = "nach Ostern"
            });
            answers.Add(new Answer()
            {
                Text = "nach dem Oktoberfest"
            });

            questions.Add(new Question()
            {
                Text = "Wann beginnt alljährlich die Starkbierzeit im Löwenbräukeller?",
                Answers = answers,
                Category = QuizCategory.LBK
            });

            //Question 12 - Frage 10
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "1806",
                Correct = true,
                Explanation = "Napoleon erhob Bayern am 1. Januar 1806 zum Königreich. Aus Kurfürst Herzog Maximilian IV. Joseph von Bayern wurde König Maximilian I. von Bayern, der sogleich München zur Hauptstadt seines neuen und größeren Reichs ernannte."
            });
            answers.Add(new Answer()
            {
                Text = "1861"
            });
            answers.Add(new Answer()
            {
                Text = "1799"
            });

            questions.Add(new Question()
            {
                Text = "Wann wurde München Hauptstadt des Königreichs Bayern?",
                Answers = answers,
                Category = QuizCategory.MUC
            });

            //Question 13 - Frage 12
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "Mistral"
            });
            answers.Add(new Answer()
            {
                Text = "Föhn",
                Correct = true,
                Explanation = "Diese Wetterlage, von den Alpen kommend, bringt einen warmen und trockenen Fallwind in die Stadt, der für eine Schön-Wetterlage sorgt."
            });
            answers.Add(new Answer()
            {
                Text = "Wetterleuchten"
            });

            questions.Add(new Question()
            {
                Text = "Welches Wetterphänomen bereitet nicht nur Münchnern mitunter Kopfschmerzen?",
                Answers = answers,
                Category = QuizCategory.MUC
            });

            //Question 14 - Frage 13
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "Welfen"
            });
            answers.Add(new Answer()
            {
                Text = "Hohenzollern"
            });
            answers.Add(new Answer()
            {
                Text = "Wittelsbacher",
                Correct = true,
                Explanation = "König Ludwig III. war der letzte Wittelsbacher auf dem Bayerischen Thron. Insgesamt 678 Jahre (1240-1918) regierten sie von München aus Bayern."
            });

            questions.Add(new Question()
            {
                Text = "Welches Adelsgeschlecht war das Letzte, dass von München aus Bayern regierte?",
                Answers = answers,
                Category = QuizCategory.BAY
            });

            //Question 15 - Frage 14
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "Heinrich den Löwe",
                Correct = true,
                Explanation = "Der Welfen-Herzog hatte seinen Hof in Braunschweig und war von 1156 bis 1180 auch Herzog von Bayern."
            });
            answers.Add(new Answer()
            {
                Text = "August der Starke"
            });
            answers.Add(new Answer()
            {
                Text = "Ludwig der Starke"
            });

            questions.Add(new Question()
            {
                Text = "Von welchem Fürsten wurde München 1158 gegründet?",
                Answers = answers,
                Category = QuizCategory.MUC
            });
            /*
            //Question 16 - Frage 15
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "Maria Theresia"
            });
            answers.Add(new Answer()
            {
                Text = "Zita Maria"
            });
            answers.Add(new Answer()
            {
                Text = "Elisabeth Amalie Eugenie",
                Correct = true,
                Explanation = "Am Heiligen Abend 1837 erblickte die Wittelsbacherin Elisabeth, die als Kaiserin \"Sissi\" weltberühmt wurde, das Licht der Welt in München. Am 10.9.1898 starb sie in Genf nach einem Attentat."
            });

            questions.Add(new Question()
            {
                Text = "Welche berühmte Österreichische Kaiserin stammte aus Bayern und wurde in München geboren?",
                Answers = answers,
                Category = QuizCategory.MUC
            });

            //Question 17 - Frage 16
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "Käse-Mix",
                Correct = true,
                Explanation = "Käsereste, wie Camembert und Weichkäse werden zusammen mit Butter, Paprika, Zwiebeln, Weißbier und anderen Gewürzen zu eine Käsecreme vermischt."
            });
            answers.Add(new Answer()
            {
                Text = "Obst-Schnaps"
            });
            answers.Add(new Answer()
            {
                Text = "Schimpfwort"
            });

            questions.Add(new Question()
            {
                Text = "Was versteht man in München unter Obatzda?",
                Answers = answers,
                Category = QuizCategory.FOD
            });

            //Question 18 - Frage 18
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "500.000",
                Correct = true,
                Explanation = "Während anno 1360 nur 10.810 Menschen innerhalb der Stadtmauern lebten, übersprang man 1901 die Halbe-Millionen-Grenze. 1957 folgte die eine Million. Und heute leben über 1,4 Millionen Menschen in der Stadt."
            });
            answers.Add(new Answer()
            {
                Text = "650.458"
            });
            answers.Add(new Answer()
            {
                Text = "400.800"
            });

            questions.Add(new Question()
            {
                Text = "Wie viele Einwohner hatte München 1901?",
                Answers = answers,
                Category = QuizCategory.MUC
            });

            //Question 19 - Frage 19
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "4"
            });
            answers.Add(new Answer()
            {
                Text = "6"
            });
            answers.Add(new Answer()
            {
                Text = "7",
                Correct = true,
                Explanation = "Anfang des 20. Jahrhunderts gab es noch über 25 Brauereien. Heute brauen noch sieben in der Stadt: Löwenbräu, Spaten, Franziskaner, Augustiner, Hacker-Pschorr, Paulaner und Hofbräu."
            });

            questions.Add(new Question()
            {
                Text = "Wie viele Münchner Biermarken findet man heute noch in der Stadt?",
                Answers = answers,
                Category = QuizCategory.BIE
            });

            //Question 20 - Frage 20
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "4"
            });
            answers.Add(new Answer()
            {
                Text = "6"
            });
            answers.Add(new Answer()
            {
                Text = "7",
                Correct = true,
                Explanation = "Anfang des 20. Jahrhunderts gab es noch über 25 Brauereien. Heute brauen noch sieben in der Stadt: Löwenbräu, Spaten, Franziskaner, Augustiner, Hacker-Pschorr, Paulaner und Hofbräu."
            });

            questions.Add(new Question()
            {
                Text = "Wie viele Münchner Biermarken findet man heute noch in der Stadt?",
                Answers = answers,
                Category = QuizCategory.FOD
            });

            //Question 21 - Frage 21
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "Olympiastadion"
            });
            answers.Add(new Answer()
            {
                Text = "U-Bahn",
                Correct = true,
                Explanation = "Mit der U6 ging die erste Münchner Untergrundbahn zwischen Kieferngarten und Goetheplatz in Betrieb. Am 8.5.1972 folgte die U3-Strecke von der Münchner Freiheit zum Olympiazentrum."
            });
            answers.Add(new Answer()
            {
                Text = "Fußgängerzone"
            });

            questions.Add(new Question()
            {
                Text = "Was wurde am 17.9.1971 in München erstmals eröffnet?",
                Answers = answers,
                Category = QuizCategory.MUC
            });

            //Question 22 - Frage 22
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "1818",
                Correct = true,
                Explanation = "Mit der konstitutionellen Monarchie wurde zwar das Zusammenspiel zwischen König und Landtag geregelt, aber die Volkssouveränität lag noch in weiter Ferne. Mit der letzten Verfassung von 1946 wurde Bayern zum Freistaat."
            });
            answers.Add(new Answer()
            {
                Text = "1946"
            });
            answers.Add(new Answer()
            {
                Text = "1919"
            });

            questions.Add(new Question()
            {
                Text = "In welchem Jahr erhielt Bayern seine erste Verfassung?",
                Answers = answers,
                Category = QuizCategory.BAY
            });

            //Question 23 - Frage 23
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "Gemüse"
            });
            answers.Add(new Answer()
            {
                Text = "Bauern"
            });
            answers.Add(new Answer()
            {
                Text = "Lebensmittel",
                Correct = true,
                Explanation = "Das Wort stammt aus dem Lateinischen und verdrängte im 19. Jahrhundert den Namen \"Grüner Markt\". König Max I. Joseph begann 1807 wegen Platzmangels mit dem Umsiedeln des Markts vom Marienplatz an seinen heutigen Ort.  "
            });

            questions.Add(new Question()
            {
                Text = "Was bedeutet das Wort \"Viktualien\", nachdem Münchens berühmtester Markt benannt ist?",
                Answers = answers,
                Category = QuizCategory.MUC
            });

            //Question 24 - Frage 24
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "Fußballweltmeisterschaft"
            });
            answers.Add(new Answer()
            {
                Text = "Olympische Sommerspiele",
                Correct = true,
                Explanation = "Die Vergabe der XX. Olympiade nach München veränderte die Stadt nicht nur rasant, sondern legte auch den Grundstein für den Erfolg des heutigen \"Millionendorfs\", wie Münchner zuweilen liebevoll ihre Stadt nennen."
            });
            answers.Add(new Answer()
            {
                Text = "Leichtathletik WM"
            });

            questions.Add(new Question()
            {
                Text = "Welches sportliche Großereignis fand 1972 vom 26.8. bis 11.9. in München statt?",
                Answers = answers,
                Category = QuizCategory.MUC
            });

            //Question 25 - Frage 25
            answers = new List<Answer>();
            answers.Add(new Answer()
            {
                Text = "Selbstverpflegung",
                Correct = true,
                Explanation = "Der Erlaß von 1812 zum Schutz der Wirtshäuser besagte, dass Biergärten und -Keller keine Speisen servieren durften. Daraus entstand die Tradition der Selbstverpflegung, die auch heute noch für jeden wahren Biergarten gilt."
            });
            answers.Add(new Answer()
            {
                Text = "Stammtische"
            });
            answers.Add(new Answer()
            {
                Text = "Maßkrug-Waschanlagen"
            });

            questions.Add(new Question()
            {
                Text = "Welche Biergarten-Tradition geht auf einen Erlaß von König Ludwig I. zurück?",
                Answers = answers,
                Category = QuizCategory.BIE
            });
            */

			return questions;
		}
	}
}

