using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Words
{
    public class groupboxMotivationalQuote :GroupBox
    {

        formWords frmWords { get { return formWords.instance; } }

            CheckBox chkAppearAtLoadTime = new CheckBox();
        public groupboxMotivationalQuote()
        {
            Text = "Motivational Quote";
            // quotes sourced from : https://smartblogger.com/writing-quotes/
            Label lblText = new Label();
            Label lblAuthor = new Label();

            Button btnOk = new Button();

            chkAppearAtLoadTime.Text = "Appear at launch";
            chkAppearAtLoadTime.AutoSize = true;
            chkAppearAtLoadTime.Checked = AppearAtLaunch;
            Controls.Add(chkAppearAtLoadTime);

            btnOk.AutoSize = true;
            btnOk.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnOk.Text = "Ok";
            btnOk.Click += BtnOk_Click;
            Controls.Add(btnOk);


            string[] strQuotes_Source =
            {
                "“You fail only if you stop writing.”— Ray Bradbury",
                "“If my doctor told me I had only six minutes to live, I wouldn’t brood. I’d type a little faster.”— Isaac Asimov",
                "“I’ve written because it fulfilled me. Maybe it paid off the mortgage on the house and got the kids through college, but those things were on the side ~ I did it for the buzz. I did it for the pure joy of the thing. And if you can do it for joy, you can do it forever.”— Stephen King",
                "“If there’s a book that you want to read, but it hasn’t been written yet, then you must write it.”— Toni Morrison",
                "“We write to taste life twice, in the moment and in retrospect.”— Anaïs Nin",
                "“Don’t bend; don’t water it down; (and) don’t try to make it logical; don’t edit your own soul according to the fashion. Rather, follow your most intense obsessions mercilessly.”— Franz Kafka",
                "“Just write every day of your life. Read intensely. Then see what happens. Most of my friends who are put on that diet have very pleasant careers.”— Ray Bradbury",
                "“When your story is ready for rewrite, cut it to the bone. Get rid of every ounce of excess fat. This is going to hurt; revising a story down to the bare essentials is always a little like murdering children, but it must be done.”— Stephen King",
                "“And by the way, everything in life is writable about if you have the outgoing guts to do it, and the imagination to improvise. The worst enemy to creativity is self-doubt.”— Sylvia Plath",
                "“How vain it is to sit down to write when you have not stood up to live.”— Henry David Thoreau",
                "“What is written without effort is in general read without pleasure.”— Samuel Johnson",
                "“90 percent perfect and shared with the world always changes more lives than 100 percent perfect and stuck in your head.”— Jon Acuff",
                "“You can’t fail if you don’t quit. You can’t succeed if you don’t start.”— Michael Hyatt",
                "“Write something that’s worth fighting over. Because that’s how you change things. That’s how you create art.”— Jeff Goins",
                "“Inspiration may sometimes fail to show up for work in the morning, but determination never does.”— K.M. Weiland",
                "“Exercise the writing muscle every day, even if it is only a letter, notes, a title list, a character sketch, a journal entry. Writers are like dancers, like athletes. Without that exercise, the muscles seize up.”— Jane Yolen",
                "“Write what disturbs you, what you fear, what you have not been willing to speak about. Be willing to be split open.”— Natalie Goldberg",
                "“Write what should not be forgotten.”— Isabel Allende",
                "“Words are a lens to focus one’s mind.”— Ayn Rand",
                "“Fill your paper with the breathings of your heart.”— William Wadsworth",
                "“You may not always write well, but you can edit a bad page. You can’t edit a blank page.”— Jodi Picoult",
                "“It took me fifteen years to discover that I had no talent for writing, but I couldn’t give it up because by that time I was too famous.”— Robert Benchley",
                "“The most beautiful things are those that madness prompts and reason writes.”— Andre Gide",
                "“Opportunities don’t happen. You create them.”— Chris Grosser",
                "“Either write something worth reading or do something worth writing.”— Benjamin Franklin",
                "“Done is better than perfect.”— Sheryl Sandberg",
                "“There’s no such thing as writer’s block. That was invented by people in California who couldn’t write.”— Terry Pratchett",
                "“You don’t start out writing good stuff. You start out writing crap and thinking it’s good stuff, and then gradually you get better at it. That’s why I say one of the most valuable traits is persistence.”— Octavia E. Butler",
                "“There is no greater agony than bearing an untold story inside you.”— Maya Angelou",
                "“Every secret of a writer’s soul, every experience of his life, every quality of his mind, is written large in his works.”— Virginia Woolf",
                "“Don’t tell me the moon is shining; show me the glint of light on broken glass.”— Anton Chekhov",
                "“No tears in the writer, no tears in the reader. No surprise in the writer, no surprise in the reader.”— Robert Frost",
                "“The first draft is just you telling yourself the story.”— Terry Pratchett",
                "“I would write a book, or a short story, at least three times ~ once to understand it, the second time to improve the prose, and a third to compel it to say what it still must say. Somewhere I put it this way: first drafts are for learning what one’s fiction wants him to say. Revision works with that knowledge to enlarge and enhance an idea, to reform it. Revision is one of the exquisite pleasures of writing.”— Bernard Malamud",
                "“The difference between the almost right word and the right word is the difference between the lightning bug and the lightning.”— Mark Twain",
                "“I love deadlines. I like the whooshing sound they make as they fly by.”— Douglas Adams",
                "“The main thing I try to do is write as clearly as I can. I rewrite a good deal to make it clear.”— E.B. White",
                "“Words can be like X-rays if you use them properly ~ they’ll go through anything. You read and you’re pierced.”— Aldous Huxley",
                "“Here is a lesson in creative writing. First rule: Do not use semicolons. (…) All they do is show you’ve been to college.”— Kurt Vonnegut Jr.",
                "“One day I will find the right words, and they will be simple.”— Jack Kerouac",
                "“When I sit down to write a book, I do not say to myself, ‘I am going to produce a work of art.’ I write it because there is some lie that I want to expose, some fact to which I want to draw attention, and my initial concern is to get a hearing.”— George Orwell",
                "“Anybody can make history. Only a great man can write it.”— Oscar Wilde",
                "“I try to leave out the parts that people skip.”— Elmore Leonard",
                "“I can shake off everything as I write; my sorrows disappear, my courage is reborn.”— Anne Frank",
                "“A person is a fool to become a writer. His only compensation is absolute freedom. He has no master except his own soul, and that, I am sure, is why he does it.”— Roald Dahl",
                "“There are three rules for writing a novel. Unfortunately, no one knows what they are.”— Somerset Maugham",
                "“I write to discover what I know.”— Flannery O’Connor",
                "“You have to write the book that wants to be written. And if the book will be too difficult for grown-ups, then you write it for children.”— Madeleine L’Engle",
                "“Writing is easy: All you do is sit staring at a blank piece of paper until drops of blood form on your forehead.”— Gene Fowler",
                "“You never have to change anything you got up in the middle of the night to write.”— Saul Bellow",
                "“Write. Rewrite. When not writing or rewriting, read. I know of no shortcuts.”— Larry L. King",
                "“I am irritated by my own writing. I am like a violinist whose ear is true, but whose fingers refuse to reproduce precisely the sound he hears within.”— Gustave Flaubert",
                "“To produce a mighty book, you must choose a mighty theme.”— Herman Melville",
                "“Good writing is rewriting.”— Truman Capote",
                "“Don’t take anyone’s writing advice too seriously.”— Lev Grossman",
                "“Cheat your landlord if you can and must, but do not try to shortchange the Muse. It cannot be done. You can’t fake quality any more than you can fake a good meal.”— William S. Burroughs",
                "“The most valuable of all talents is that of never using two words when one will do.”— Thomas Jefferson",
                "“The greatest part of a writer’s time is spent in reading, in order to write; a man will turn over half a library to make one book.”— Samuel Johnson",
                "“Any man who keeps working is not a failure. He may not be a great writer, but if he applies the old-fashioned virtues of hard, constant labor, he’ll eventually make some kind of career for himself as writer.”— Ray Bradbury",
                "“Do not hoard what seems good for a later place in the book, or for another book; give it, give it all, give it now.”— Annie Dillard",
                "“You can make anything by writing.”— C.S. Lewis",
                "“You don’t write because you want to say something, you write because you have something to say.”— F. Scott Fitzgerald",
                "“Some editors are failed writers, but so are most writers.”— T.S. Eliot",
                "“I wake up in the morning and my mind starts making sentences, and I have to get rid of them fast ~ talk them or write them down.”— Ernest Hemingway",
                "“Empty your knapsack of all adjectives, adverbs and clauses that slow your stride and weaken your pace. Travel light.”— Bill Moyers",
                "“If I waited for perfection, I would never write a word.”— Margaret Atwood",
                "“Everybody walks past a thousand story ideas every day. The good writers are the ones who see five or six of them. Most people don’t see any.”— Orson Scott Card",
                "“Start writing, no matter what. The water does not flow until the faucet is turned on.”— Louis L’Amour",
                "“A writer needs three things, experience, observation, and imagination, any two of which, at times any one of which, can supply the lack of the others.”— William Faulkner",
                "“A professional writer is an amateur who didn’t quit.”— Richard Bach",
                "“All you have to do is write one true sentence. Write the truest sentence that you know.”— Ernest Hemingway",
                "“You have to resign yourself to wasting lots of trees before you write anything really good. That’s just how it is. It’s like learning an instrument. You’ve got to be prepared for hitting wrong notes occasionally, or quite a lot. That’s just part of the learning process.”— J.K. Rowling",
                "“Failures are finger posts on the road to achievement.”— C. S. Lewis",
                "“A writer is someone for whom writing is more difficult than it is for other people.”— Thomas Mann",
                "“(…) write your story as it needs to be written. Write it ­honestly, and tell it as best you can. I’m not sure that there are any other rules. Not ones that matter.”— Neil Gaiman",
                "“Don’t try to figure out what other people want to hear from you; figure out what you have to say. It’s the one and only thing you have to offer.”— Barbara Kingsolver",
                "“When you are pouring yourself into your work and bringing your unique perspective and skills to the table, then you are adding value that only you are capable of contributing.”— Todd Henry",
                "“Writing a novel is like driving a car at night. You can only see as far as your headlights, but you can make the whole trip that way.”— E. L. Doctorow",
                "“We were born to be brave.”— Bob Goff",
                "“Almost all good writing begins with terrible first efforts. You need to start somewhere.”— Anne Lamott",
                "“I could write an entertaining novel about rejection slips, but I fear it would be overly long.”— Louise Brown",
                "“Ideas are like rabbits. You get a couple and learn how to handle them, and pretty soon you have a dozen.”— John Steinbeck",
                "“I went for years not finishing anything. Because, of course, when you finish something you can be judged.”— Erica Jong",
                "“If the book is true, it will find an audience that is meant to read it.”— Wally Lamb",
                "“People say, ‘What advice do you have for people who want to be writers?’ I say, they don’t really need advice, they know they want to be writers, and they’re gonna do it. Those people who know that they really want to do this and are cut out for it, they know it.”— R.L. Stine",
                "“Most writers draw a blank when they first start with writing prompts. Keep pushing through, because something thrilling will start to happen.”— Mel Wicks",
                "“It’s none of their business that you have to learn to write. Let them think you were born that way.”— Ernest Hemingway",
                "“I do not over-intellectualise the production process. I try to keep it simple: Tell the d*mned story.”— Tom Clancy",
                "“Being a writer is not just about typing. It’s also about surviving the rollercoaster of the creative journey.”— Joanna Penn",
                "“Success is no accident. It is hard work, perseverance, learning, studying, sacrifice and most of all, love of what you are doing or learning to do.”— Pele",
                "“Don’t count the days, make the days count.”— Muhammad Ali",
                "“Failure will never overtake me if my determination to succeed is strong enough.”— Og Mandino",
                "“Hard work beats talent when talent doesn’t work hard.”— Tim Notke",
                "“Live as if you were to die tomorrow. Learn as if you were to live forever.”— Mahatma Gandhi",
                "“Too many of us are not living our dreams because we are living our fears.”— Les Brown",
                "“I do not think that there is any other quality so essential to success of any kind as the quality of perseverance. It overcomes almost everything, even nature.”— John D. Rockefeller",
                "“A goal is not always meant to be reached; it often serves simply as something to aim at.”— Bruce Lee",
                "“I’m not a product of my circumstances. I am a product of my decisions.”— Stephen Covey",
                "“There is only one thing that makes a dream impossible to achieve, the fear of failure.”— Paulo Coelho",
                "“The whole secret of a successful life is to find out what is one’s destiny to do, and then do it.”— Henry Ford",
                "“If you are not willing to risk the usual you will have to settle for the ordinary.”— Jim Rohn",
                "“Perfection is not attainable, but if we chase perfection we can catch excellence.”— Vince Lombardi",
                "“Failure is another stepping stone to greatness.”— Oprah Winfrey",
                "“The best way to gain self-confidence is to do what you are afraid to do.”— Swati Sharma",
                "“The only way to do great work is to love what you do. If you haven’t found it yet, keep looking. Don’t settle.”— Steve Jobs",
                "“Unsuccessful people make their decisions based on their current situations. Successful people make their decisions based on where they want to be.”— Benjamin Hardy",
                "“Success is going from failure to failure without losing your enthusiasm.”— Winston Churchill",
                "“You were born to win, but to be a winner, you must plan to win, prepare to win, and expect to win.”— Zig Ziglar",
                "“A person who never made a mistake never tried anything new.”— Albert Einstein",
                "“Learn from the mistakes of others. You can’t live long enough to make them all yourself.”— Eleanor Roosevelt",
                "“Do what you can with all you have, wherever you are.”— Theodore Roosevelt",
                "“All our dreams can come true, if we have the courage to pursue them.”— Walt Disney",
                "“Our greatest glory is not in never falling, but in rising every time we fall.”— Confucius",
                "“Don’t say you don’t have enough time. You have exactly the same number of hours per day that were given to Helen Keller, Pasteur, Michelangelo, Mother Teresa, Leonardo Da Vinci, Thomas Jefferson, and Albert Einstein.”— H. Jackson Brown Jr.",
                "“What you lack in talent can be made up with desire, hustle and giving 110% all the time.”— Don Zimmer",
                "“Do the best you can. No one can do more than that.”— John Wooden",
                "“What we fear of doing most is usually what we most need to do.”— Ralph Waldo Emerson",
                "“If you believe it’ll work out, you’ll see opportunities. If you don’t believe it’ll work out, you’ll see obstacles.”— Wayne Dyer",
                "“Don’t worry when you are not recognized, but strive to be worthy of recognition.”— Abraham Lincoln",
                "“We are what we repeatedly do. Excellence, then, is not an act, but a habit.”— Aristotle",
                "“One important key to success is self-confidence. An important key to self-confidence is preparation.”— Arthur Ashe",
                "“Success is a lousy teacher. It seduces smart people into thinking they can’t lose.”— Bill Gates",
                "“Move out of your comfort zone. You can only grow if you are willing to feel awkward and uncomfortable when you try something new.”— Brian Tracy",
                "“Just one small positive thought in the morning can change your whole day.”— Dalai Lama",
                "“Develop success from failures. Discouragement and failure are two of the surest stepping stones to success.”— Dale Carnegie",
                "“You must expect great things of yourself before you can do them.”— Michael Jordan",
                "“If you cannot do great things, do small things in a great way.”— Napoleon Hill",
                "“Everything you’ve ever wanted is on the other side of fear.”— George Addair",
                "“The path to success is to take massive, determined action.”— Tony Robbins",
                "“Tough times never last, but tough people do.”— Robert Schuller",
                "“Only put off until tomorrow what you are willing to die having left undone.”— Pablo Picasso",
                "“Don’t watch the clock; do what it does. Keep going.”— Sam Levenson",
                "“Our greatest weakness lies in giving up. The most certain way to succeed is always to try just one more time.”— Thomas Edison",
                "“Give your dreams all you’ve got and you’ll be amazed at the energy that comes out of you.”— William James",
                "“It’s hard to beat a person who never gives up.”— Babe Ruth",
                "“Failure after long perseverance is much grander than never to have a striving good enough to be called a failure.”— George Eliot",
                "“I have not failed. I’ve just found 10,000 ways that won’t work.”— Thomas A. Edison"
            };
            do
            {
                Random rnd = new Random();
                int intRND = (int)(rnd.NextDouble() * int.MaxValue) % strQuotes_Source.Length;
                string strQuote = strQuotes_Source[intRND];

                int intCut = strQuote.IndexOf("—");
                if (intCut >=0)
                {
                    string strText = strQuote.Substring(0, intCut);
                    string strAuthor = strQuote.Substring(intCut);

                    lblText.Font = new Font("arial", 12);
                    lblText.ForeColor = Color.Blue;
                    lblAuthor.Font = new Font("Arial", 10);
                    lblAuthor.AutoSize = true;

                    BackColor = Color.LightGray;

                    Size szText = TextRenderer.MeasureText(strText, lblText.Font);

                    float fltArea = szText.Width * szText.Height;
                    float fltSquare =(float)Math.Sqrt(fltArea);
                    fltSquare *= 1.1f;
                    if (fltSquare < 200)
                        fltSquare = 200;

                    lblText.Width = (int)(fltSquare * 2f);
                    lblText.Height = (int)(fltSquare * .6f);

                    Controls.Add(lblText);
                    Controls.Add(lblAuthor);
                    lblText.Text = strText;
                    lblAuthor.Text = strAuthor;

                    lblText.Location = new Point(15, 25);
                    lblAuthor.Location = new Point(lblText.Right - lblAuthor.Width /2, lblText.Bottom+5);

                    chkAppearAtLoadTime.Location = new Point(lblText.Left, lblAuthor.Bottom + 5);
                    btnOk.Location = new Point(lblAuthor.Right - btnOk.Width, chkAppearAtLoadTime.Top);

                    Size = new Size(lblAuthor.Right + lblText.Left, btnOk.Bottom + 15);

                    frmWords.Controls.Add(this);
                    Location = new Point((frmWords.Width - Width) / 2, (frmWords.Height - Height) / 2);
                    Show();
                    BringToFront();
                    return;
                }
            } while (true);
        }

        static bool bolAppearAtLaunch = false;
        public static bool AppearAtLaunch
        {
            get { return bolAppearAtLaunch; }
            set { bolAppearAtLaunch = value; }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            AppearAtLaunch = chkAppearAtLoadTime.Checked;
            Hide();
            Dispose();
        }
    }
}
