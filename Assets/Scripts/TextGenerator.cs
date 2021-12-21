using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TextGenerator : MonoBehaviour
{
    private static readonly string[] NewLeaderTitlesMale = { "Mister", "Master", "Admiral", "Archbishop", "Attorney General", "Baron", "Bishop", "Brother", "Canon", "Captain", "Cardinal", "Chief", "Colonel", "Commander", "Corporal", "Chancellor", "Count", "Doctor", "Duke", "Father", "General", "Justice", "Laird", "Lieutenant", "Lieutenant Colonel", "Lieutenant Commander", "Lord", "Major", "Pastor", "Private", "Proffessor", "Rabbi", "Reverand", "Sergeant", "Sir", "Viscount" };

    private static readonly string[] NewLeaderTitlesFemale = { "Missus", "Miss", "Admiral", "Archbishop", "Attorney General", "Baroness", "Bishop", "Canon", "Captain", "Cardinal", "Chief", "Colonel", "Commander", "Corporal", "Chancellor", "Countess", "Dame", "Doctor", "Duchess", "Earl", "Father", "General", "Justice", "Lady", "Laird", "Lieutenant", "Lieutenant Colonel", "Lieutenant Commander", "Madam", "Major", "Pastor", "Private", "Proffessor", "Rabbi", "Reverand", "Sergeant", "Sister", "Viscountess" };

    private static readonly string[] NewLeaderFirstNamesMale = { "Liam", "Noah", "William", "James", "Oliver", "Benjamin", "Elijah", "Lucas", "Mason", "Logan", "Alexander", "Ethan", "Jacob", "Michael", "Daniel", "Henry", "Jackson", "Sebastian", "Aiden", "Matthew", "Samuel", "David", "Joseph", "Carter", "Owen", "Wyatt", "John", "Jack", "Luke", "Jayden", "Dylan", "Grayson", "Levi", "Isaac", "Gabriel", "Julian", "Mateo", "Anthony", "Jaxon", "Lincoln", "Joshua", "Christopher", "Andrew", "Theodore", "Caleb", "Ryan", "Asher", "Nathan", "Thomas", "Leo", "Isaiah", "Charles", "Josiah", "Hudson", "Christian", "Hunter", "Connor", "Eli", "Ezra", "Aaron", "Landon", "Adrian", "Jonathan", "Nolan", "Jeremiah", "Easton", "Elias", "Colton", "Cameron", "Carson", "Robert", "Angel", "Maverick", "Nicholas", "Dominic", "Jaxson", "Greyson", "Adam", "Ian", "Austin", "Santiago", "Jordan", "Cooper", "Brayden", "Roman", "Evan", "Ezekiel", "Xavier", "Jose", "Jace", "Jameson", "Leonardo", "Bryson", "Axel", "Everett", "Parker", "Kayden", "Miles", "Sawyer", "Jason", "Declan", "Weston", "Micah", "Ayden", "Wesley", "Luca", "Vincent", "Damian", "Zachary", "Silas", "Gavin", "Chase", "Kai", "Emmett", "Harrison", "Nathaniel", "Kingston", "Cole", "Tyler", "Bennett", "Bentley", "Ryker", "Tristan", "Brandon", "Kevin", "Luis", "George", "Ashton", "Rowan", "Braxton", "Ryder", "Gael", "Ivan", "Diego", "Maxwell", "Max", "Carlos", "Kaiden", "Juan", "Maddox", "Justin", "Waylon", "Calvin", "Giovanni", "Jonah", "Abel", "Jayce", "Jesus", "Amir", "King", "Beau", "Camden", "Alex", "Jasper", "Malachi", "Brody", "Jude", "Blake", "Emmanuel", "Eric", "Brooks", "Elliot", "Antonio", "Abraham", "Timothy", "Finn", "Rhett", "Elliott", "Edward", "August", "Xander", "Alan", "Dean", "Lorenzo", "Bryce", "Karter", "Victor", "Milo", "Miguel", "Hayden", "Graham", "Grant", "Zion", "Tucker", "Jesse", "Zayden", "Joel", "Richard", "Patrick", "Emiliano", "Avery", "Nicolas", "Brantley", "Dawson", "Myles", "Matteo", "River", "Steven", "Thiago", "Zane", "Matias", "Judah", "Messiah", "Jeremy", "Preston", "Oscar", "Kaleb", "Alejandro", "Marcus", "Mark", "Peter", "Maximus", "Barrett", "Jax", "Andres", "Holden", "Legend", "Charlie", "Knox", "Kaden", "Paxton", "Kyrie", "Kyle", "Griffin", "Josue", "Kenneth", "Beckett", "Enzo", "Adriel", "Arthur", "Felix", "Bryan", "Lukas", "Paul", "Brian", "Colt", "Caden", "Leon", "Archer", "Omar", "Israel", "Aidan", "Theo", "Javier", "Remington", "Jaden", "Bradley", "Emilio", "Colin", "Riley", "Cayden", "Phoenix", "Clayton", "Simon", "Ace", "Nash", "Derek", "Rafael", "Zander", "Brady", "Jorge", "Jake", "Louis", "Damien", "Karson", "Walker", "Maximiliano", "Amari", "Sean", "Chance", "Walter", "Martin", "Finley", "Andre", "Tobias", "Cash", "Corbin", "Arlo", "Iker", "Erick", "Emerson", "Gunner", "Cody", "Stephen", "Francisco", "Killian", "Dallas", "Reid", "Manuel", "Lane", "Atlas", "Rylan", "Jensen", "Ronan", "Beckham", "Daxton", "Anderson", "Kameron", "Raymond", "Orion", "Cristian", "Tanner", "Kyler", "Jett", "Cohen", "Ricardo", "Spencer", "Gideon", "Ali", "Fernando", "Jaiden", "Titus", "Travis", "Bodhi", "Eduardo", "Dante", "Ellis", "Prince", "Kane", "Luka", "Kash", "Hendrix", "Desmond", "Donovan", "Mario", "Atticus", "Cruz", "Garrett", "Hector", "Angelo", "Jeffrey", "Edwin", "Cesar", "Zayn", "Devin", "Conor", "Warren", "Odin", "Jayceon", "Romeo", "Julius", "Jaylen", "Hayes", "Kayson", "Muhammad", "Jaxton", "Joaquin", "Caiden", "Dakota", "Major", "Keegan", "Sergio", "Marshall", "Johnny", "Kade", "Edgar", "Leonel", "Ismael", "Marco", "Tyson", "Wade", "Collin", "Troy", "Nasir", "Conner", "Adonis", "Jared", "Rory", "Andy", "Jase", "Lennox", "Shane", "Malik", "Ari", "Reed", "Seth", "Clark", "Erik", "Lawson", "Trevor", "Gage", "Nico", "Malakai", "Quinn", "Cade", "Johnathan", "Sullivan", "Solomon", "Cyrus", "Fabian", "Pedro", "Frank", "Shawn", "Malcolm", "Khalil", "Nehemiah", "Dalton", "Mathias", "Jay", "Ibrahim", "Peyton", "Winston", "Kason", "Zayne", "Noel", "Princeton", "Matthias", "Gregory", "Sterling", "Dominick", "Elian", "Grady", "Russell", "Finnegan", "Ruben", "Gianni", "Porter", "Kendrick", "Leland", "Pablo", "Allen", "Hugo", "Raiden", "Kolton", "Remy", "Ezequiel", "Damon", "Emanuel", "Zaiden", "Otto", "Bowen", "Marcos", "Abram", "Kasen", "Franklin", "Royce", "Jonas", "Sage", "Philip", "Esteban", "Drake", "Kashton", "Roberto", "Harvey", "Alexis", "Kian", "Jamison", "Maximilian", "Adan", "Milan", "Phillip", "Albert", "Dax", "Mohamed", "Ronin", "Kamden", "Hank", "Memphis", "Oakley", "Augustus", "Drew", "Moises", "Armani", "Rhys", "Benson", "Jayson", "Kyson", "Braylen", "Corey", "Gunnar", "Omari", "Alonzo", "Landen", "Armando", "Derrick", "Dexter", "Enrique", "Bruce", "Nikolai", "Francis", "Rocco", "Kairo", "Royal", "Zachariah", "Arjun", "Deacon", "Skyler", "Eden", "Alijah", "Rowen", "Pierce", "Uriel", "Ronald", "Luciano", "Tate", "Frederick", "Kieran", "Lawrence", "Moses", "Rodrigo", "Brycen", "Leonidas", "Nixon", "Keith", "Chandler", "Case", "Davis", "Asa", "Darius", "Isaias", "Aden", "Jaime", "Landyn", "Raul", "Niko", "Trenton", "Apollo", "Cairo", "Izaiah", "Scott", "Dorian", "Julio", "Wilder", "Santino", "Dustin", "Donald", "Raphael", "Saul", "Taylor", "Ayaan", "Duke", "Ryland", "Tatum", "Ahmed", "Moshe", "Edison", "Emmitt", "Cannon", "Alec", "Danny", "Keaton", "Roy", "Conrad", "Roland", "Quentin", "Lewis", "Samson", "Brock", "Kylan", "Cason", "Ahmad", "Jalen", "Nikolas", "Braylon", "Kamari", "Dennis", "Callum", "Justice", "Soren", "Rayan", "Aarav", "Gerardo", "Ares", "Brendan", "Jamari", "Kaison", "Yusuf", "Issac", "Jasiah", "Callen", "Forrest", "Makai", "Crew", "Kobe", "Bo", "Julien", "Mathew", "Braden", "Johan", "Marvin", "Zaid", "Stetson", "Casey", "Ty", "Ariel", "Tony", "Zain", "Callan", "Cullen", "Sincere", "Uriah", "Dillon", "Kannon", "Colby", "Axton", "Cassius", "Quinton", "Mekhi", "Reece", "Alessandro", "Jerry", "Mauricio", "Sam", "Trey", "Mohammad", "Alberto", "Gustavo", "Arturo", "Fletcher", "Marcelo", "Abdiel", "Hamza", "Alfredo", "Chris", "Finnley", "Curtis", "Kellan", "Quincy", "Kase", "Harry", "Kyree", "Wilson", "Cayson", "Hezekiah", "Kohen", "Neil", "Mohammed", "Raylan", "Kaysen", "Lucca", "Sylas", "Mack", "Leonard", "Lionel", "Ford", "Roger", "Rex", "Alden", "Boston", "Colson", "Briggs", "Zeke", "Dariel", "Kingsley", "Valentino", "Jamir", "Salvador", "Vihaan", "Mitchell", "Lance", "Lucian", "Darren", "Jimmy", "Alvin", "Amos", "Tripp", "Zaire", "Layton", "Reese", "Casen", "Colten", "Brennan", "Korbin", "Sonny", "Bruno", "Orlando", "Devon", "Huxley", "Boone", "Maurice", "Nelson", "Douglas", "Randy", "Gary", "Lennon", "Titan", "Denver", "Jaziel", "Noe", "Jefferson", "Ricky", "Lochlan", "Rayden", "Bryant", "Langston", "Lachlan", "Clay", "Abdullah", "Lee", "Baylor", "Leandro", "Ben", "Kareem", "Layne", "Joe", "Crosby", "Deandre", "Demetrius", "Kellen", "Carl", "Jakob", "Ridge", "Bronson", "Jedidiah", "Rohan", "Larry", "Stanley", "Tomas", "Shiloh", "Thaddeus", "Watson", "Baker", "Vicente", "Koda", "Jagger", "Nathanael", "Carmelo", "Shepherd", "Graysen", "Melvin", "Ernesto", "Jamie", "Yosef", "Clyde", "Eddie", "Tristen", "Grey", "Ray", "Tommy", "Samir", "Ramon", "Santana", "Kristian", "Marcel", "Wells", "Zyaire", "Brecken", "Byron", "Otis", "Reyansh", "Axl", "Joey", "Trace", "Morgan", "Musa", "Harlan", "Enoch", "Henrik", "Kristopher", "Talon", "Rey", "Guillermo", "Houston", "Jon", "Vincenzo", "Dane", "Terry", "Azariah", "Castiel", "Kye", "Augustine", "Zechariah", "Joziah", "Kamryn", "Hassan", "Jamal", "Chaim", "Bodie", "Emery", "Branson", "Jaxtyn", "Kole", "Wayne", "Aryan", "Alonso", "Brixton", "Madden", "Allan", "Flynn", "Jaxen", "Harley", "Magnus", "Sutton", "Dash", "Anders", "Westley", "Brett", "Emory", "Felipe", "Yousef", "Jadiel", "Mordechai", "Dominik", "Junior", "Eliseo", "Fisher", "Harold", "Jaxxon", "Kamdyn", "Maximo", "Caspian", "Kelvin", "Damari", "Fox", "Trent", "Hugh", "Briar", "Franco", "Keanu", "Terrance", "Yahir", "Ameer", "Kaiser", "Thatcher", "Ishaan", "Koa", "Merrick", "Coen", "Rodney", "Brayan", "London", "Rudy", "Gordon", "Bobby", "Aron", "Marc", "Van", "Anakin", "Canaan", "Dario", "Reginald", "Westin", "Darian", "Ledger", "Leighton", "Maxton", "Tadeo", "Valentin", "Aldo", "Khalid", "Nickolas", "Toby", "Dayton", "Jacoby", "Billy", "Gatlin", "Elisha", "Jabari", "Jermaine", "Alvaro", "Marlon", "Mayson", "Blaze", "Jeffery", "Kace", "Braydon", "Achilles", "Brysen", "Saint", "Xzavier", "Aydin", "Eugene", "Adrien", "Cain", "Kylo", "Nova", "Onyx", "Arian", "Bjorn", "Jerome", "Miller", "Alfred", "Kenzo", "Kyng", "Leroy", "Maison", "Jordy", "Stefan", "Wallace", "Benicio", "Kendall", "Zayd", "Blaine", "Tristian", "Anson", "Gannon", "Jeremias", "Marley", "Ronnie", "Dangelo", "Kody", "Will", "Bentlee", "Gerald", "Salvatore", "Turner", "Chad", "Misael", "Mustafa", "Konnor", "Maxim", "Rogelio", "Zakai", "Cory", "Judson", "Brentley", "Darwin", "Louie", "Ulises", "Dakari", "Rocky", "Wesson", "Alfonso", "Payton", "Dwayne", "Juelz", "Duncan", "Keagan", "Deshawn", "Bode", "Bridger", "Skylar", "Brodie", "Landry", "Avi", "Keenan", "Reuben", "Jaxx", "Rene", "Yehuda", "Imran", "Yael", "Alexzander", "Willie", "Cristiano", "Heath", "Lyric", "Davion", "Elon", "Karsyn", "Krew", "Jairo", "Maddux", "Ephraim", "Ignacio", "Vivaan", "Aries", "Vance", "Boden", "Lyle", "Ralph", "Reign", "Camilo", "Draven", "Terrence", "Idris", "Ira", "Javion", "Jericho", "Khari", "Marcellus", "Creed", "Shepard", "Terrell", "Ahmir", "Camdyn", "Cedric", "Howard", "Jad", "Zahir", "Harper", "Justus", "Forest", "Gibson", "Zev", "Alaric", "Decker", "Ernest", "Jesiah", "Torin", "Benedict", "Bowie", "Deangelo", "Genesis", "Harlem", "Kalel", "Kylen", "Bishop", "Immanuel", "Lian", "Zavier", "Archie", "Davian", "Gus", "Kabir", "Korbyn", "Randall", "Benton", "Coleman", "Markus" };

    private static readonly string[] NewLeaderFirstNamesFemale = { "Emma", "Olivia", "Ava", "Isabella", "Sophia", "Charlotte", "Mia", "Amelia", "Harper", "Evelyn", "Abigail", "Emily", "Elizabeth", "Mila", "Ella", "Avery", "Sofia", "Camila", "Aria", "Scarlett", "Victoria", "Madison", "Luna", "Grace", "Chloe", "Penelope", "Layla", "Riley", "Zoey", "Nora", "Lily", "Eleanor", "Hannah", "Lillian", "Addison", "Aubrey", "Ellie", "Stella", "Natalie", "Zoe", "Leah", "Hazel", "Violet", "Aurora", "Savannah", "Audrey", "Brooklyn", "Bella", "Claire", "Skylar", "Lucy", "Paisley", "Everly", "Anna", "Caroline", "Nova", "Genesis", "Emilia", "Kennedy", "Samantha", "Maya", "Willow", "Kinsley", "Naomi", "Aaliyah", "Elena", "Sarah", "Ariana", "Allison", "Gabriella", "Alice", "Madelyn", "Cora", "Ruby", "Eva", "Serenity", "Autumn", "Adeline", "Hailey", "Gianna", "Valentina", "Isla", "Eliana", "Quinn", "Nevaeh", "Ivy", "Sadie", "Piper", "Lydia", "Alexa", "Josephine", "Emery", "Julia", "Delilah", "Arianna", "Vivian", "Kaylee", "Sophie", "Brielle", "Madeline", "Peyton", "Rylee", "Clara", "Hadley", "Melanie", "Mackenzie", "Reagan", "Adalynn", "Liliana", "Aubree", "Jade", "Katherine", "Isabelle", "Natalia", "Raelynn", "Maria", "Athena", "Ximena", "Arya", "Leilani", "Taylor", "Faith", "Rose", "Kylie", "Alexandra", "Mary", "Margaret", "Lyla", "Ashley", "Amaya", "Eliza", "Brianna", "Bailey", "Andrea", "Khloe", "Jasmine", "Melody", "Iris", "Isabel", "Norah", "Annabelle", "Valeria", "Emerson", "Adalyn", "Ryleigh", "Eden", "Emersyn", "Anastasia", "Kayla", "Alyssa", "Juliana", "Charlie", "Esther", "Ariel", "Cecilia", "Valerie", "Alina", "Molly", "Reese", "Aliyah", "Lilly", "Parker", "Finley", "Morgan", "Sydney", "Jordyn", "Eloise", "Trinity", "Daisy", "Kimberly", "Lauren", "Genevieve", "Sara", "Arabella", "Harmony", "Elise", "Remi", "Teagan", "Alexis", "London", "Sloane", "Laila", "Lucia", "Diana", "Juliette", "Sienna", "Elliana", "Londyn", "Ayla", "Callie", "Gracie", "Josie", "Amara", "Jocelyn", "Daniela", "Everleigh", "Mya", "Rachel", "Summer", "Alana", "Brooke", "Alaina", "Mckenzie", "Catherine", "Amy", "Presley", "Journee", "Rosalie", "Ember", "Brynlee", "Rowan", "Joanna", "Paige", "Rebecca", "Ana", "Sawyer", "Mariah", "Nicole", "Brooklynn", "Payton", "Marley", "Fiona", "Georgia", "Lila", "Harley", "Adelyn", "Alivia", "Noelle", "Gemma", "Vanessa", "Journey", "Makayla", "Angelina", "Adaline", "Catalina", "Alayna", "Julianna", "Leila", "Lola", "Adriana", "June", "Juliet", "Jayla", "River", "Tessa", "Lia", "Dakota", "Delaney", "Selena", "Blakely", "Ada", "Camille", "Zara", "Malia", "Hope", "Samara", "Vera", "Mckenna", "Briella", "Izabella", "Hayden", "Raegan", "Michelle", "Angela", "Ruth", "Freya", "Kamila", "Vivienne", "Aspen", "Olive", "Kendall", "Elaina", "Thea", "Kali", "Destiny", "Amiyah", "Evangeline", "Cali", "Blake", "Elsie", "Juniper", "Alexandria", "Myla", "Ariella", "Kate", "Mariana", "Lilah", "Charlee", "Daleyza", "Nyla", "Jane", "Maggie", "Zuri", "Aniyah", "Lucille", "Leia", "Melissa", "Adelaide", "Amina", "Giselle", "Lena", "Camilla", "Miriam", "Millie", "Brynn", "Gabrielle", "Sage", "Annie", "Logan", "Lilliana", "Haven", "Jessica", "Kaia", "Magnolia", "Amira", "Adelynn", "Makenzie", "Stephanie", "Nina", "Phoebe", "Arielle", "Evie", "Lyric", "Alessandra", "Gabriela", "Paislee", "Raelyn", "Madilyn", "Paris", "Makenna", "Kinley", "Gracelyn", "Talia", "Maeve", "Rylie", "Kiara", "Evelynn", "Brinley", "Jacqueline", "Laura", "Gracelynn", "Lexi", "Ariah", "Fatima", "Jennifer", "Kehlani", "Alani", "Ariyah", "Luciana", "Allie", "Heidi", "Maci", "Phoenix", "Felicity", "Joy", "Kenzie", "Veronica", "Margot", "Addilyn", "Lana", "Cassidy", "Remington", "Saylor", "Ryan", "Keira", "Harlow", "Miranda", "Angel", "Amanda", "Daniella", "Royalty", "Gwendolyn", "Ophelia", "Heaven", "Jordan", "Madeleine", "Esmeralda", "Kira", "Miracle", "Elle", "Amari", "Danielle", "Daphne", "Willa", "Haley", "Gia", "Kaitlyn", "Oakley", "Kailani", "Winter", "Alicia", "Serena", "Nadia", "Aviana", "Demi", "Jada", "Braelynn", "Dylan", "Ainsley", "Alison", "Camryn", "Avianna", "Bianca", "Skyler", "Scarlet", "Maddison", "Nylah", "Sarai", "Regina", "Dahlia", "Nayeli", "Raven", "Helen", "Adrianna", "Averie", "Skye", "Kelsey", "Tatum", "Kensley", "Maliyah", "Erin", "Viviana", "Jenna", "Anaya", "Carolina", "Shelby", "Sabrina", "Mikayla", "Annalise", "Octavia", "Lennon", "Blair", "Carmen", "Yaretzi", "Kennedi", "Mabel", "Zariah", "Kyla", "Christina", "Selah", "Celeste", "Eve", "Mckinley", "Milani", "Frances", "Jimena", "Kylee", "Leighton", "Katie", "Aitana", "Kayleigh", "Sierra", "Kathryn", "Rosemary", "Jolene", "Alondra", "Elisa", "Helena", "Charleigh", "Hallie", "Lainey", "Avah", "Jazlyn", "Kamryn", "Mira", "Cheyenne", "Francesca", "Antonella", "Wren", "Chelsea", "Amber", "Emory", "Lorelei", "Nia", "Abby", "April", "Emelia", "Carter", "Aylin", "Cataleya", "Bethany", "Marlee", "Carly", "Kaylani", "Emely", "Liana", "Madelynn", "Cadence", "Matilda", "Sylvia", "Myra", "Fernanda", "Oaklyn", "Elianna", "Hattie", "Dayana", "Kendra", "Maisie", "Malaysia", "Kara", "Katelyn", "Maia", "Celine", "Cameron", "Renata", "Jayleen", "Charli", "Emmalyn", "Holly", "Azalea", "Leona", "Alejandra", "Bristol", "Collins", "Imani", "Meadow", "Alexia", "Edith", "Kaydence", "Leslie", "Lilith", "Kora", "Aisha", "Meredith", "Danna", "Wynter", "Emberly", "Julieta", "Michaela", "Alayah", "Jemma", "Reign", "Colette", "Kaliyah", "Elliott", "Johanna", "Remy", "Sutton", "Emmy", "Virginia", "Briana", "Oaklynn", "Adelina", "Everlee", "Megan", "Angelica", "Justice", "Mariam", "Khaleesi", "Macie", "Karsyn", "Alanna", "Aleah", "Mae", "Mallory", "Esme", "Skyla", "Madilynn", "Charley", "Allyson", "Hanna", "Shiloh", "Henley", "Macy", "Maryam", "Ivanna", "Ashlynn", "Lorelai", "Amora", "Ashlyn", "Sasha", "Baylee", "Beatrice", "Itzel", "Priscilla", "Marie", "Jayda", "Liberty", "Rory", "Alessia", "Alaia", "Janelle", "Kalani", "Gloria", "Sloan", "Dorothy", "Greta", "Julie", "Zahra", "Savanna", "Annabella", "Poppy", "Amalia", "Zaylee", "Cecelia", "Coraline", "Kimber", "Emmie", "Anne", "Karina", "Kassidy", "Kynlee", "Monroe", "Anahi", "Jaliyah", "Jazmin", "Maren", "Monica", "Siena", "Marilyn", "Reyna", "Kyra", "Lilian", "Jamie", "Melany", "Alaya", "Ariya", "Kelly", "Rosie", "Adley", "Dream", "Jaylah", "Laurel", "Jazmine", "Mina", "Karla", "Bailee", "Aubrie", "Katalina", "Melina", "Harlee", "Elliot", "Hayley", "Elaine", "Karen", "Dallas", "Irene", "Lylah", "Ivory", "Chaya", "Rosa", "Aleena", "Braelyn", "Nola", "Alma", "Leyla", "Pearl", "Addyson", "Roselyn", "Lacey", "Lennox", "Reina", "Aurelia", "Noa", "Janiyah", "Jessie", "Madisyn", "Saige", "Alia", "Tiana", "Astrid", "Cassandra", "Kyleigh", "Romina", "Stevie", "Haylee", "Zelda", "Lillie", "Aileen", "Brylee", "Eileen", "Yara", "Ensley", "Lauryn", "Giuliana", "Livia", "Anya", "Mikaela", "Palmer", "Lyra", "Mara", "Marina", "Kailey", "Liv", "Clementine", "Kenna", "Briar", "Emerie", "Galilea", "Tiffany", "Bonnie", "Elyse", "Cynthia", "Frida", "Kinslee", "Tatiana", "Joelle", "Armani", "Jolie", "Nalani", "Rayna", "Yareli", "Meghan", "Rebekah", "Addilynn", "Faye", "Zariyah", "Lea", "Aliza", "Julissa", "Lilyana", "Anika", "Kairi", "Aniya", "Noemi", "Angie", "Crystal", "Bridget", "Ari", "Davina", "Amelie", "Amirah", "Annika", "Elora", "Xiomara", "Linda", "Hana", "Laney", "Mercy", "Hadassah", "Madalyn", "Louisa", "Simone", "Kori", "Jillian", "Alena", "Malaya", "Miley", "Milan", "Sariyah", "Malani", "Clarissa", "Nala", "Princess", "Amani", "Analia", "Estella", "Milana", "Aya", "Chana", "Jayde", "Tenley", "Zaria", "Itzayana", "Penny", "Ailani", "Lara", "Aubriella", "Clare", "Lina", "Rhea", "Bria", "Thalia", "Keyla", "Haisley", "Ryann", "Addisyn", "Amaia", "Chanel", "Ellen", "Harmoni", "Aliana", "Tinsley", "Landry", "Paisleigh", "Lexie", "Myah", "Rylan", "Deborah", "Emilee", "Laylah", "Novalee", "Ellis", "Emmeline", "Avalynn", "Hadlee", "Legacy", "Braylee", "Elisabeth", "Kaylie", "Ansley", "Dior", "Paula", "Belen", "Corinne", "Maleah", "Martha", "Teresa", "Salma", "Louise", "Averi", "Lilianna", "Amiya", "Milena", "Royal", "Aubrielle", "Calliope", "Frankie", "Natasha", "Kamilah", "Meilani", "Raina", "Amayah", "Lailah", "Rayne", "Zaniyah", "Isabela", "Nathalie", "Miah", "Opal", "Kenia", "Azariah", "Hunter", "Tori", "Andi", "Keily", "Leanna", "Scarlette", "Jaelyn", "Saoirse", "Selene", "Dalary", "Lindsey", "Marianna", "Ramona", "Estelle", "Giovanna", "Holland", "Nancy", "Emmalynn", "Mylah", "Rosalee", "Sariah", "Zoie", "Blaire", "Lyanna", "Maxine", "Anais", "Dana", "Judith", "Kiera", "Jaelynn", "Noor", "Kai", "Adalee", "Oaklee", "Amaris", "Jaycee", "Belle", "Carolyn", "Della", "Karter", "Sky", "Treasure", "Vienna", "Jewel", "Rivka", "Rosalyn", "Alannah", "Ellianna", "Sunny", "Claudia", "Cara", "Hailee", "Estrella", "Harleigh", "Zhavia", "Alianna", "Brittany", "Jaylene", "Journi", "Marissa", "Mavis", "Iliana", "Jurnee", "Aislinn", "Alyson", "Elsa", "Kamiyah", "Kiana", "Lisa", "Arlette", "Kadence", "Kathleen", "Halle", "Erika", "Sylvie", "Adele", "Erica", "Veda", "Whitney", "Bexley", "Emmaline", "Guadalupe", "August", "Brynleigh", "Gwen", "Promise", "Alisson", "India", "Madalynn", "Paloma", "Patricia", "Samira", "Aliya", "Casey", "Jazlynn", "Paulina", "Dulce", "Kallie", "Perla", "Adrienne", "Alora", "Nataly", "Ayleen", "Christine", "Kaiya", "Ariadne", "Karlee", "Barbara", "Lillianna", "Raquel", "Saniyah", "Yamileth", "Arely", "Celia", "Heavenly", "Kaylin", "Marisol", "Marleigh", "Avalyn", "Berkley", "Kataleya", "Zainab", "Dani", "Egypt", "Joyce", "Kenley", "Annabel", "Kaelyn", "Etta", "Hadleigh", "Joselyn", "Luella", "Jaylee", "Zola", "Alisha", "Ezra", "Queen", "Amia", "Annalee", "Bellamy", "Paola", "Tinley", "Violeta", "Jenesis", "Arden", "Giana", "Wendy", "Ellison", "Florence", "Margo", "Naya", "Robin", "Sandra", "Scout", "Waverly", "Janessa", "Jayden", "Micah", "Novah", "Zora", "Ann", "Jana", "Taliyah", "Vada", "Giavanna", "Ingrid", "Valery", "Azaria", "Emmarie", "Esperanza", "Kailyn" };

    private static readonly string[] NewLeaderLastNames = { "Chung", "Chen", "Melton", "Hill", "Puckett", "Song", "Hamilton", "Bender", "Wagner", "McLaughlin", "McNamara", "Raynor", "Moon", "Woodard", "Desai", "Wallace", "Lawrence", "Griffin", "Dougherty", "Powers", "May", "Steele", "Teague", "Vick", "Gallagher", "Solomon", "Walsh", "Monroe", "Connolly", "Hawkins", "Middleton", "Goldstein", "Watts", "Johnston", "Weeks", "Wilkerson", "Barton", "Walton", "Hall", "Ross", "Chung", "Bender", "Woods", "Mangum", "Joseph", "Rosenthal", "Bowden", "Barton", "Underwood", "Jones", "Baker", "Merritt", "Cross", "Cooper", "Holmes", "Sharpe", "Morgan", "Hoyle", "Allen", "Rich", "Rich", "Grant", "Proctor", "Diaz", "Graham", "Watkins", "Hinton", "Marsh", "Hewitt", "Branch", "Walton", "O'Brien", "Case", "Watts", "Christensen", "Parks", "Hardin", "Lucas", "Eason", "Davidson", "Whitehead", "Rose", "Sparks", "Moore", "Pearson", "Rodgers", "Graves", "Scarborough", "Sutton", "Sinclair", "Bowman", "Olsen", "Love", "McLean", "Christian", "Lamb", "James", "Chandler", "Stout", "Cowan", "Golden", "Bowling", "Beasley", "Clapp", "Abrams", "Tilley", "Morse", "Boykin", "Sumner", "Cassidy", "Davidson", "Heath", "Blanchard", "McAllister", "McKenzie", "Byrne", "Schroeder", "Griffin", "Gross", "Perkins", "Robertson", "Palmer", "Brady", "Rowe", "Zhang", "Hodge", "Li", "Bowling", "Justice", "Glass", "Willis", "Hester", "Floyd", "Graves", "Fischer", "Norman", "Chan", "Hunt", "Byrd", "Lane", "Kaplan", "Heller", "May", "Jennings", "Hanna", "Locklear", "Holloway", "Jones", "Glover", "Vick", "O'Donnell", "Goldman", "McKenna", "Starr", "Stone", "McClure", "Watson", "Monroe", "Abbott", "Singer", "Hall", "Farrell", "Lucas", "Norman", "Atkins", "Monroe", "Robertson", "Sykes", "Reid", "Chandler", "Finch", "Hobbs", "Adkins", "Kinney", "Whitaker", "Alexander", "Conner", "Waters", "Becker", "Rollins", "Love", "Adkins", "Black", "Fox", "Hatcher", "Wu", "Lloyd", "Joyce", "Welch", "Matthews", "Chappell", "MacDonald", "Kane", "Butler", "Pickett", "Bowman", "Barton", "Kennedy", "Branch", "Thornton", "McNeill", "Weinstein", "Middleton", "Moss", "Lucas", "Rich", "Carlton", "Brady", "Schultz", "Nichols", "Harvey", "Stevenson", "Houston", "Dunn", "West", "O'Brien", "Barr", "Snyder", "Cain", "Heath", "Boswell", "Olsen", "Pittman", "Weiner", "Petersen", "Davis", "Coleman", "Terrell", "Norman", "Burch", "Weiner", "Parrott", "Henry", "Gray", "Chang", "McLean", "Eason", "Weeks", "Siegel", "Puckett", "Heath", "Hoyle", "Garrett", "Neal", "Baker", "Goldman", "Shaffer", "Choi", "Carver", "Shelton", "House", "Lyons", "Moser", "Dickinson", "Abbott", "Hobbs", "Dodson", "Spencer", "Burgess", "Liu", "Wong", "Blackburn", "McKay", "Middleton", "Frazier", "Reid", "Braswell", "Steele", "Donovan", "Barrett", "Nance", "Washington", "Rogers", "McMahon", "Miles", "Kramer", "Jennings", "Bowles", "Brown", "Bolton", "Craven", "Hendrix", "Nichols", "Saunders", "Lehman", "Sherrill", "Cash", "Pittman", "Sullivan", "Whitehead", "Mack", "Rice", "Ayers", "Cherry", "Richmond", "York", "Wiley", "Harrington", "Reed", "Nash", "Wilkerson", "Kent", "Finch", "Starr", "Holland", "Glover", "Clements", "Schultz", "Hawley", "Skinner", "Hamrick", "Winters", "Dolan", "Turner", "Beatty", "Douglas", "Byrne", "Hendricks", "Mayer", "Cochran", "Reilly", "Jensen", "Yates", "Haynes", "Harmon", "Matthews", "Dawson", "Barefoot", "Kaplan", "Gross", "Richmond", "Pope", "Pickett", "Schwartz", "Singleton", "Ballard", "Spivey", "Denton", "Huff", "Mangum", "Berger", "McCall", "Pollard", "Garcia", "Wagner", "Crane", "Wolf", "Crane", "Dalton", "Diaz", "Currin", "Stanton", "Carey", "Li", "Chan", "Hess", "Robinson", "Mills", "Bender", "McDonald", "Moore", "Fox", "Lanier", "Harris", "Underwood", "Parsons", "Vaughn", "Banks", "Sherrill", "Oakley", "Rubin", "Maynard", "Hill", "Livingston", "Lam", "Thompson", "Creech", "Dillon", "Foster", "Starr", "Roy", "Barbour", "Burke", "Ritchie", "Odom", "Pearce", "Rosenberg", "Garrett", "O'Connor", "Cates", "McIntosh", "Olson", "Cox", "Erickson", "Chang", "Briggs", "Klein", "Goldberg", "Hinson", "Weiss", "Pritchard", "Goldman", "Lassiter", "Massey", "Stark", "Dunlap", "Humphrey", "Singleton", "Horowitz", "Lutz", "Hoover", "Kang", "Melton", "Teague", "Ellington", "Cherry", "Jennings", "Creech", "Lynn", "Albright", "Alston", "Burnette", "O'Neal", "Morris", "Lutz", "Callahan", "Conway", "Harvey", "Watson", "Glover", "Savage", "Henson", "Wang", "Ellis", "Barbour", "Sherrill", "Pierce", "Woodward", "Godfrey", "Langston", "Eaton", "Lowe", "Stanton", "Fuller", "Simmons", "Schultz", "Knight", "Klein", "Garcia", "Schroeder", "Hess", "Gold", "Hensley", "Turner", "French", "Hughes", "Pate", "Burnett", "Francis", "Horn", "Forrest", "Levin", "Weiner", "Durham", "Guthrie", "Hensley", "Freedman", "Wiggins", "Best", "Beatty", "Crawford", "Drake", "Curtis", "Walter", "Dunlap", "Jenkins", "Hood", "Ellis", "Jiang", "Johnson", "Craig", "Norman", "McIntyre", "Brantley", "Kelley", "Smith", "Lyons", "Wall", "Quinn", "Hicks", "Garrison", "Watts", "Dickerson", "Waller", "Carter", "Robinson", "Katz", "Hull", "Bowling", "Brantley", "Brock", "James", "McMillan", "Hu", "Waller", "Abbott", "McKee", "Waters", "Sims", "Henderson", "Rao", "Bray", "Scarborough", "Ford", "Blum", "Kenney", "Gordon", "Blair", "Moore", "Kemp", "Hutchinson", "Brennan", "Little", "Gill", "Keller", "Rosenthal", "McConnell", "Sawyer", "McCall", "Coates", "Hicks", "Davidson", "Hawkins", "Lindsay", "Gonzalez", "Gray", "English", "Duke", "Webb", "Baldwin", "Lamb", "Shaffer", "Wang", "Burgess", "Smith", "Fletcher", "Boyd", "Hirsch", "Currie", "McKenzie", "Weber", "Honeycutt", "Manning", "Bolton", "Ritchie", "Baldwin", "Riley", "Swanson", "Huffman", "Gibson", "Yates", "Wrenn", "Green", "Harris", "Hayes", "Hamrick", "Hawley", "Koch", "McKenzie", "Harrell", "Parsons", "McGuire", "Stephenson", "Baxter", "Summers", "Welch", "Nixon", "Kelly", "Sumner", "Cobb", "Bruce", "Newton", "Rogers", "Sanchez", "Finch", "Silverman", "Horn", "Richardson", "Gay", "Chase", "Gallagher", "Kern", "Scott", "Bradley", "Puckett", "Sanchez", "Yang", "Brantley", "Bunn", "Link", "Nguyen", "Stephens", "Horne", "Burton", "Diaz", "Berry", "Knowles", "Freeman", "Hernandez", "Roach", "Hardison", "Wolf", "Boyd", "Caldwell", "Mann", "McLeod", "Stanton", "Park", "Chang", "Newton", "Phillips", "Whitaker", "Pitts", "McLean", "Barton", "Gould", "Atkins", "Shapiro", "Vincent", "Harrell", "Boswell", "Lassiter", "Fisher", "Case", "Parsons", "McPherson", "Wiley", "Schwartz", "McFarland", "Baker", "Holden", "Hartman", "Schwartz", "Nguyen", "Houston", "Friedman", "Adcock", "Stephens", "McClure", "Proctor", "Lang", "Berger", "Aldridge", "Davies", "Wall", "Miles", "Bolton", "Morgan", "Fisher", "Stephens", "Holmes", "Ferrell", "Henry", "Hedrick", "Horne", "Weiss", "Singh", "Blalock", "Aldridge", "Ritchie", "Grossman", "Pugh", "Olson", "Fernandez", "Arnold", "Stanley", "Field", "Farmer", "Jernigan", "Bowers", "Crabtree", "Crabtree", "Clements", "Spivey", "Archer", "Owen", "Strickland", "Berg", "Gibbons", "Warner", "Bray", "Eason", "Hoover", "Park", "Anderson", "Li", "Elmore", "Pearson", "Harper", "Chu", "Schultz", "Black", "Mitchell", "Sharp", "Glover", "Cates", "Martin", "Lowry", "Cooke", "Fink", "Barrett", "Olson", "Melton", "Coley", "Mueller", "Paul", "Daniel", "Padgett", "Daniels", "Hayes", "Hines", "Pridgen", "Stone", "Hayes", "Harris", "Walter", "Woods", "Jennings", "Lopez", "McCarthy", "Frederick", "Lopez", "Scarborough", "Brandt", "Nolan", "Chandler", "Carlton", "Katz", "Parrott", "Corbett", "Godfrey", "Cooke", "Pate", "Barber", "Fletcher", "Schroeder", "Lindsay", "Boswell", "Buckley", "Harmon", "Walters", "Stevens", "Knight", "Rowland", "Lindsay", "Bowling", "Kirby", "Benson", "Anthony", "Dunn", "Hill", "Lang", "Grimes", "Bowers", "Bowden", "Underwood", "Zhang", "Godwin", "Rice", "Townsend", "Lin", "Pitts", "Koch", "Callahan", "Long", "Norton", "Blackburn", "O'Connell", "Bowling", "Robinson", "Pritchard", "Lawson", "Dickerson", "Livingston", "Hansen", "Berman", "Carroll", "Kearney", "Peterson", "Richards", "Sutherland", "McCormick", "Beach", "Wu", "Hunt", "Carver", "Anthony", "Livingston", "Floyd", "McCall", "Haynes", "Gunter", "Solomon", "Harris", "Cline", "McKay", "Braun", "Preston", "Hayes", "Burnette", "Finch", "Levine", "Lynch", "Simpson", "Galloway", "Dickson", "Murphy", "Cannon", "Fleming", "Hanson", "Blackwell", "Zimmerman", "Dyer", "Greenberg", "Quinn", "Sullivan", "Stanley", "Hendrix", "Barber", "High", "Pickett", "Copeland", "Beck", "McKenna", "King", "Stone", "Benton", "Boyette", "Byers", "Cook", "Nixon", "Mayo", "Hardison", "Marks", "Ball", "Kirk", "Cooke", "Sutton", "Gibson", "Haynes", "Klein", "Tyson", "Payne", "Francis", "Roth", "Nixon", "Coble", "Walters", "Hewitt", "Langley", "Scott", "Willis", "Denton", "Daly", "Lam", "Fox", "Franklin", "McIntosh", "Tyler", "Hanna", "Davenport", "Barton", "Chambers", "Thomas", "Arthur", "Law", "Coley", "Vaughn", "Case", "Reed", "Hardy", "Beatty", "Dale", "Russell", "Whitley", "Curry", "McNeill", "Franklin", "Lindsay", "Casey", "Meadows", "Casey", "Love", "Fitzpatrick", "Mann", "Knowles", "Hale", "Carlson", "Barefoot", "Warren", "Nelson", "Lancaster", "Kay", "Burgess", "Fitzpatrick", "Davies", "Moran", "Ashley", "Caldwell", "Kelley", "Mack", "Reilly", "Copeland", "Love", "Conrad", "Padgett", "Poole", "McKinney", "Sawyer", "Dalton", "Carey", "Stuart", "Bowles", "Singleton", "Britt", "Owens", "Davenport", "Cox", "Barton", "Cooke", "Tilley", "Pugh", "Schultz", "Connor", "Herbert", "Aycock", "Barry", "Bishop", "Garrett", "Bailey", "Riddle", "Sawyer", "Burnett", "Boyette", "McKenzie", "Sinclair", "Cannon", "Freeman", "Wallace", "Gilbert", "McNamara", "Mullen", "Bradshaw", "Hinson", "Jordan", "Berger", "Upchurch", "Bowers", "Allison", "Alexander", "Coley", "Riley", "O'Brien", "Vaughan", "Hartman", "Chung", "Fischer", "Sellers", "Montgomery", "Snow", "McKnight", "McMahon", "Chu", "Crews", "Sharma", "Puckett", "Pappas", "Sharpe", "Olson", "Desai" };

    public static string LeaderName()
    {
        List<string[]> nameSets = new List<string[]>();
        bool isMale = (Random.Range(0, 2) == 1);

        if (Random.Range(0, 5) >= 2) // 60% chance for title
        {
            nameSets.Add((isMale) ? NewLeaderTitlesMale : NewLeaderTitlesFemale);
            if (Random.Range(0, 2) == 1)
                nameSets.Add(NewLeaderLastNames);
            else
            {
                nameSets.Add((Random.Range(0, 2) == 1) ? NewLeaderFirstNamesMale : NewLeaderFirstNamesFemale);
                if (Random.Range(0, 2) == 1)
                    nameSets.Add(NewLeaderLastNames);
            }
        }
        else // 40% for no title
        {
            nameSets.Add((isMale) ? NewLeaderFirstNamesMale : NewLeaderFirstNamesFemale);
            if (Random.Range(0, 5) >= 1) // 80% chance for a last name            
                nameSets.Add(NewLeaderLastNames);
        }

        string newName = "";
        nameSets.ForEach(x => newName += DevTools.RandomListValue<string>(x.ToList()) + " ");
        return newName.Trim(" ".ToCharArray());
    }
}
