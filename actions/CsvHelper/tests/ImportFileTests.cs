using System.Reflection;
using System.Text;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;
using Xunit;
using UkrGuru.Extensions;

namespace CsvHelperTests
{
    public class ImportFileTests
    {
        private readonly bool dbOK = false;

        public ImportFileTests()
        {
            var dbName = "CsvHelperTest";

            var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True";

            DbHelper.ConnectionString = connectionString.Replace(dbName, "master");

            DbHelper.ExecCommand($"IF DB_ID('{dbName}') IS NULL CREATE DATABASE {dbName};");

            DbHelper.ConnectionString = connectionString;

            if (dbOK) return;

            var assembly1 = Assembly.GetAssembly(typeof(UkrGuru.WebJobs.Actions.BaseAction));
            ArgumentNullException.ThrowIfNull(assembly1);
            dbOK = assembly1.InitDb();

            var assembly2 = Assembly.GetAssembly(typeof(UkrGuru.WebJobs.Actions.CsvHelper.ImportFileAction));
            ArgumentNullException.ThrowIfNull(assembly2);
            dbOK &= assembly2.InitDb();

            var assembly3 = Assembly.GetAssembly(typeof(ImportFileTests));
            ArgumentNullException.ThrowIfNull(assembly3);
            dbOK &= assembly3.InitDb();
        }

        [Fact]
        public void InitDbTest()
        {
            Assert.True(dbOK);
        }

        [Fact]
        public async Task ImportFileTestAsync()
        {
            var csv = @"seq,firstname,lastname,age,street,city,state,zip,pick,date
1,Connor,Sherman,56,Rokefo Key,Egepizrug,CO,30432,YELLOW,02/19/2052
2,Gene,Hamilton,62,Zepceh Pike,Noepaif,WV,69281,YELLOW,03/05/1927
3,Calvin,Hayes,38,Upor Lane,Gozkifo,MA,18626,WHITE,02/06/1996
4,Glen,Morgan,28,Edora Trail,Macwucus,SC,36594,YELLOW,11/10/2009
5,Marian,Gardner,64,Vebum Extension,Hufcivle,AZ,24091,BLUE,05/05/2037
6,Ann,Lucas,51,Rebhit Mill,Kasalo,NJ,48335,WHITE,03/13/1911
7,Alex,Pearson,35,Gegir Loop,Saivod,MT,36683,YELLOW,02/19/1937
8,Nettie,Wheeler,30,Gakew Place,Winosca,NM,85886,WHITE,07/16/2053
9,Jim,Sanchez,37,Iztoz Pass,Nusfotej,PA,07494,GREEN,12/04/1917
10,Floyd,Marsh,37,Feco Mill,Dovlaju,IA,72023,RED,04/23/1986
11,Bess,Goodman,49,Wugta Avenue,Opawitic,VA,66255,YELLOW,05/27/1972
12,Angel,Morgan,61,Woeh Center,Zahavafu,RI,59687,WHITE,03/24/1949
13,Mable,Hart,40,Juri Heights,Avveron,MO,70955,RED,06/27/1900
14,Zachary,Greer,36,Juwit Parkway,Irworo,HI,00815,GREEN,03/25/1933
15,Nathaniel,Pratt,47,Areje Place,Vogonor,TX,50322,YELLOW,10/15/1911
16,Lura,Stanley,23,Ulte Junction,Kisebur,SC,32115,GREEN,11/29/2050
17,Janie,Myers,37,Pugon Key,Loudgep,PA,78415,YELLOW,12/24/2023
18,Mae,Hudson,25,Uwne Junction,Udideraw,NE,82249,BLUE,06/03/1985
19,Lettie,Watkins,62,Omovec Way,Bidemze,LA,30456,GREEN,11/11/2019
20,Lulu,Rose,18,Igupom Heights,Edzigig,VA,58726,RED,12/02/1910
21,Victor,Rodriquez,35,Jefe Terrace,Vovkatgip,SC,22914,RED,08/10/1942
22,Elizabeth,Ward,38,Falu Junction,Cesumek,OR,62934,BLUE,01/19/1979
23,Abbie,Tyler,57,Wunvah Pike,Pefwavig,DC,58141,BLUE,11/07/2045
24,Kate,Ramsey,49,Lewate Pike,Wudojga,MT,88789,YELLOW,06/27/2011
25,Ryan,Davis,54,Furo View,Sejonumep,GA,80451,WHITE,06/21/1936
26,Jeremiah,Scott,39,Mafe Center,Gutavjan,NV,96106,RED,05/03/2041
27,Jeremy,Collier,31,Wubdel Place,Pelwomfum,ND,81018,RED,04/24/1968
28,Sam,Rice,63,Tevu Path,Weceoj,MT,45664,BLUE,01/18/2012
29,Chris,Baldwin,62,Sugre Grove,Vubwoga,WV,16311,RED,05/31/1918
30,Julian,Padilla,18,Noca Square,Atezupva,RI,69383,BLUE,09/28/1950
31,Jeanette,Knight,63,Suut Mill,Hililnal,CA,08841,WHITE,04/23/2015
32,Vernon,Jordan,37,Guhu Street,Samsuub,NJ,59255,GREEN,08/27/2063
33,Steven,Fuller,59,Ilivu Path,Zufnajba,MT,87737,BLUE,03/13/1919
34,Trevor,Turner,60,Awizid Center,Konampa,VA,29744,RED,11/29/2037
35,Andrew,Fleming,31,Elcu Road,Ilohesza,TN,80618,GREEN,06/07/1982
36,Carrie,Morgan,22,Pepsas Pass,Zucugukof,DC,73032,RED,09/21/1979
37,Austin,Sandoval,62,Ciju Plaza,Sefweipo,SC,31363,GREEN,07/19/1999
38,Adeline,Ramirez,29,Miciz Drive,Ivavebte,MS,88799,WHITE,08/12/1958
39,Nina,Gonzales,29,Toba Terrace,Bopwuvok,OH,99497,WHITE,08/18/2047
40,Alan,Henderson,44,Patuc Street,Fesehuz,MO,59786,RED,01/08/1907
41,Cole,Osborne,28,Wubti Mill,Cehviuli,VA,93979,BLUE,06/06/1914
42,Christian,Young,25,Torhe Junction,Pavuwasir,OK,28444,GREEN,05/02/1997
43,Corey,Burton,30,Netwe Junction,Wabohat,HI,10715,YELLOW,10/01/2041
44,Inez,Cannon,41,Miwtiv Heights,Roguwpeg,MA,48605,WHITE,08/18/2025
45,Henry,Conner,25,Kefon Turnpike,Jenamapi,LA,06854,WHITE,09/10/2036
46,Bernard,Schmidt,33,Degeh Road,Dinauda,OR,20547,YELLOW,11/07/1958
47,Sally,French,43,Wefwu Point,Takjikgec,OK,77943,BLUE,12/18/1943
48,Polly,Morales,56,Habo Road,Bupfeum,OH,91254,GREEN,02/26/1999
49,Kevin,Lucas,30,Zetol Plaza,Secukus,DE,44149,WHITE,12/28/2030
50,Seth,Joseph,52,Sutu Plaza,Sonasliv,KS,18604,WHITE,01/19/1990
51,Birdie,Alexander,48,Wiej Pike,Potubwig,VT,96737,BLUE,02/11/2043
52,Kate,May,34,Hobus Heights,Gakkive,MT,04605,WHITE,12/27/2044
53,Katherine,Collins,20,Fifzic Circle,Visolpov,TX,71601,GREEN,03/07/1992
54,Milton,Edwards,20,Furaf Way,Parvojij,WA,60269,WHITE,06/30/2061
55,Gussie,Cole,25,Masa View,Kimtiija,MS,19168,BLUE,04/26/1918
56,Mamie,Fuller,60,Udiel Loop,Ipajuzo,NJ,84014,BLUE,07/13/1999
57,Jeremiah,Erickson,40,Sopmi Drive,Ninnerov,VA,01755,RED,08/04/1968
58,Caroline,Young,29,Hanek Key,Fiminit,OH,45392,BLUE,01/25/2050
59,Nelle,Lane,31,Tesja Way,Dinindil,DC,05464,YELLOW,03/07/1995
60,Amy,Becker,42,Zunug Center,Ladageej,PA,71191,YELLOW,09/09/1994
61,Jeremiah,Salazar,43,Banu Manor,Pokerigu,WA,50736,GREEN,09/13/2037
62,Ollie,Barnett,41,Umru Manor,Bupada,MT,70406,BLUE,07/09/1903
63,Alma,Morris,53,Telis Drive,Awezoded,NM,76445,GREEN,05/16/2035
64,Bettie,Mathis,34,Nekgo Grove,Okvoer,GA,82666,YELLOW,04/07/1943
65,Esther,Salazar,35,Ilani Pike,Hocohum,NH,85030,WHITE,09/03/1990
66,Lee,Wilkins,32,Osipe Junction,Kerrimtuv,ME,40134,GREEN,11/25/2017
67,Mabelle,Curtis,63,Adhi Drive,Bedigus,MS,88632,GREEN,02/09/2046
68,Thomas,Sparks,65,Cama Loop,Mawkodlar,FL,92231,GREEN,10/07/1972
69,Victoria,Carpenter,47,Zujpig Path,Efvufza,MS,80847,WHITE,07/21/1955
70,Nannie,Williams,30,Ties Park,Reenib,ND,23489,RED,03/24/1973
71,Ethel,King,39,Satne Avenue,Kacvazko,IN,44932,RED,03/14/2035
72,Lester,Bradley,56,Birji Grove,Kireas,WY,34236,RED,01/30/1926
73,Harriett,Peters,28,Vucum Trail,Lufcatu,AK,44939,YELLOW,10/13/1943
74,Alberta,Taylor,61,Ruaz Turnpike,Fobzupiz,MA,97016,RED,04/01/1943
75,Polly,Stokes,44,Adil Path,Cidinhiv,MD,31463,RED,05/15/2030
76,Dylan,Maldonado,65,Migej Mill,Ehessih,CO,96464,RED,01/01/2032
77,Derrick,Santiago,20,Foknu Junction,Neppelev,PA,57149,GREEN,10/07/1948
78,Tom,May,40,Wukeka Center,Icacepa,WY,39364,GREEN,05/13/2006
79,Adeline,Harris,57,Upuwi Junction,Juzemsi,WA,54862,WHITE,12/09/1959
80,Christine,Love,61,Wata Extension,Pozrajze,MS,55279,WHITE,05/26/1956
81,Sadie,Briggs,29,Effa Pass,Ubraju,NY,31913,WHITE,10/14/1907
82,Chester,Rose,19,Nahuvu Manor,Rafjennuh,AL,76363,GREEN,01/07/1999
83,Teresa,Adams,44,Cepset Heights,Pauvilol,VA,87657,YELLOW,09/22/2016
84,Earl,Andrews,20,Egagu Parkway,Fonrefro,OH,72350,BLUE,07/17/1957
85,Ian,Gonzalez,50,Fonheg Parkway,Hupnufju,AL,73475,RED,06/19/1918
86,Lou,Brewer,46,Kecel Street,Ofiilri,IL,85444,GREEN,12/26/1915
87,Hunter,Fuller,50,Ijfek Ridge,Uhibiteh,VA,03613,YELLOW,09/15/2034
88,Billy,Wilkerson,23,Vuwha Drive,Hodnobug,NY,26547,YELLOW,04/16/2050
89,Isabelle,Nguyen,53,Lezvev Center,Josahicu,ID,50690,RED,10/03/1910
90,Tommy,Benson,25,Isado Point,Ditatvol,VT,06977,BLUE,10/13/2048
91,Lenora,Mathis,44,Imisu Highway,Kesila,ME,98556,BLUE,09/23/1933
92,Luke,Bell,21,Josuz Mill,Divafu,WI,91768,WHITE,09/25/1929
93,Luella,Ryan,62,Gewu Lane,Tourda,NH,06667,RED,04/20/1966
94,Willie,Jenkins,59,Ijgi Turnpike,Hiscehito,WV,48119,WHITE,06/27/2051
95,Anne,Vega,61,Kator Grove,Zahhivuk,DC,97656,BLUE,05/10/2001
96,Robert,Graves,42,Kocsaz Street,Okigubib,PA,44397,GREEN,12/28/2028
97,Don,Daniel,40,Zura Avenue,Laerdit,MI,42840,YELLOW,07/30/1932
98,Sam,Moreno,49,Fafup Grove,Lasige,FL,95922,RED,12/21/1973
99,Maurice,Singleton,45,Jivi View,Wemuzuta,NH,58566,BLUE,08/06/1987
100,Barbara,Thomas,60,Hasoh River,Fokukte,IA,96921,RED,06/19/1995
";

            var wjbFile = new WJbFile() { FileName = "customers.csv", FileContent = Encoding.UTF8.GetBytes(csv) };

            var guidFile = await wjbFile.SetAsync();

            Assert.NotNull(guidFile);

            var jobId = await DbHelper.FromProcAsync<int>("WJbQueue_Ins", new
            {
                Rule = 40,  /* Import Csv File */
                RulePriority = (byte)Priorities.ASAP,
                RuleMore = new { file = guidFile }
            });

            TestJob(jobId);

            Assert.True(true);
        }

        static void TestRule(int ruleId)
        {
            var jobId = DbHelper.FromProc<int>("WJbRules_Test", ruleId);

            TestJob(jobId);
        }

        static void TestJob(int jobId)
        {
            var job = DbHelper.FromProc<JobQueue>("WJbQueue_Start", jobId.ToString());

            if (job?.JobId > 0)
            {
                bool result = false;
                try
                {
                    var action = job.CreateAction();

                    if (action != null)
                    {
                        result = action.ExecuteAsync().Result;

                        // action.NextAsync(result).Wait();
                    }
                }
                catch
                {
                    result = false;
                    throw;
                }
                finally
                {
                    DbHelper.ExecProc("WJbQueue_Finish", new { JobId = jobId, JobStatus = result ? JobStatus.Completed : JobStatus.Failed });
                }
            }
        }
    }
}