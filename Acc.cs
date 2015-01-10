using System;
using System.Text;

class Acc
{
    private static void RandUpdate()
    {
        if (++_randCallCounter > RandUpdatePeriod)
        {
            _rnd = new ThreadSafeRandom();
            _randCallCounter = 0;
        }
    }
    public static int Pow(int num, int pow)
    {
        int result = 1;
        for (int i = 0; i < pow; ++i)
            result *= num;
        return result;
    }
    public static string Transliterate(string input)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char ch in input)
        {
            for (int i = 0; i < LettersCount; ++i)
            {
                if (ch == Cyr[i])
                {
                    sb.Append(Lat[i]);
                    break;
                }
            }
        }
        return sb.ToString();
    }

    public static char GetRandomChar(bool vowel)
    {
        RandUpdate();
        return vowel ? Vowels[_rnd.Next(VowelsCount)] : Consonants[_rnd.Next(ConsonantsCount)];
    }

    /// <param name="sex">'m' = male, 'f' = female, any other = random</param>
    /// <param name="second">Second Name</param>
    public static string GetRandomName(char sex = 'r', bool second = false)
    {
        RandUpdate();
        bool female;
        if (sex == 'f') female = true;
        else if (sex == 'm') female = false;
        else female = _rnd.Next(2) == 1;

        int index = second ? 1 : 0;
        return female
            ? RuFemaleNames[index][_rnd.Next(RuFemaleNamesCount[index])]
            : RuMaleNames[index][_rnd.Next(RuMaleNamesCount[index])];
    }

    public static DateTime GetRandDateTime(int minAge = 19, int maxAge = 40)
    {
        RandUpdate();
        DateTime todayDate = DateTime.Today;
        DateTime startDate = todayDate.AddYears(-maxAge - 1);
        DateTime endDate = todayDate.AddYears(-minAge);
        int range = (endDate - startDate).Days;
        return startDate.AddDays(_rnd.Next(range));
    }

    /// <summary>
    /// Get random Letter Combination login.
    /// </summary>
    /// <param name="minLength"></param>
    /// <param name="maxLength"></param>
    public static string GetRandomLogin(int minLength = 6, int maxLength = 10)
    {
        if (maxLength < minLength)
            throw new Exception("Wrong min/max length");

        RandUpdate();

        // Just to make login shorter than 10 symbols in most 4/5 cases
        if (_rnd.Next(5) != 0)
        {
            maxLength = (minLength + maxLength) / 2 + ((maxLength - minLength >= 2) ? 1 : 0);
        }
        return GetRandomPassword(_rnd.Next(minLength, maxLength));
    }

    /// <summary>Generate random login: </summary> 
    /// <param name="mode">
    /// <list type="table">
    /// <item><description>0 - Random</description></item> 
    /// <item><description>1 - FirstSecond</description></item> 
    /// <item><description>2 - FSecond</description></item> 
    /// <item><description>3 - FirstS</description></item> 
    /// <item><description>4 - SecondFirst</description></item> 
    /// <item><description>5 - SFirst</description></item> 
    /// <item><description>6 - SecondF</description></item> 
    /// <item><description>7 - Random Letter Comnination</description></item> 
    /// </list> 
    /// </param>
    /// <param name="name"></param>
    /// <param name="secName"></param>
    /// <param name="randLetterCombChance">Chance of getting Random Letter Combination Option</param>
    /// <param name="minLength"></param>
    /// <param name="maxLength"></param>
    /// <param name="minRandLetterCombLength">Min length of login if it's a rand letter combination</param>
    /// <param name="maxRandLetterCombLength">Max length of login if it's a rand letter combination</param>
    public static string GetRandomLogin(string name, string secName, int mode = 0, int randLetterCombChance = 3, int minLength = 6, int maxLength = 25, int minRandLetterCombLength = 6, int maxRandLetterCombLength = 10)
    {
        if (maxLength < minLength)
            throw new Exception("Wrong min/max length");

        RandUpdate();
        string divider = (_rnd.Next(2) == 1) ? "-" : "";
        string login;
        if (mode == 0) mode = _rnd.Next(1, 7 + randLetterCombChance);
        switch (mode)
        {
            case 1:
                login = Transliterate(name) + divider + Transliterate(secName);
                break;
            case 2:
                login = Transliterate(name[0].ToString()) + Transliterate(secName);
                break;
            case 3:
                login = Transliterate(name) + Transliterate(secName[0].ToString());
                break;
            case 4:
                login = Transliterate(secName) + divider + Transliterate(name);
                break;
            case 5:
                login = Transliterate(secName[0].ToString()) + Transliterate(name);
                break;
            case 6:
                login = Transliterate(secName) + Transliterate(name[0].ToString());
                break;
            default:
                login = GetRandomPassword(_rnd.Next(minRandLetterCombLength, maxRandLetterCombLength));
                break;
        }

        if (mode > 6)
            return login.ToLower();

        int loginLength = login.Length;
        if (loginLength > maxLength)
        {
            login = login.Remove(login.Length - (loginLength - maxLength));
            return login.ToLower();
        }

        // Appending numbers to login
        int minNumCount = (minLength - loginLength) > 0 ? (minLength - loginLength) : 0;
        int maxNumCount = maxLength - loginLength;

        int prefmMxNumCount = _rnd.Next(4);

        if (minNumCount == 0)
            maxNumCount = maxNumCount <= prefmMxNumCount ? maxNumCount : prefmMxNumCount;
        else
            maxNumCount = (minNumCount + prefmMxNumCount) < maxNumCount ? (minNumCount + prefmMxNumCount) : maxNumCount;

        int x = Pow(10, minNumCount), y = Pow(10, maxNumCount);
        login += _rnd.Next(x, y);

        return login.ToLower();
    }

    public static string GetRandomPassword(int length = 8)
    {
        const int minNumCount = 0;
        const int maxNumCount = 3;

        RandUpdate();
        //Random rnd = new Random();
        int digitCount = _rnd.Next(minNumCount, maxNumCount);
        StringBuilder sb = new StringBuilder();
        bool vowel = _rnd.Next(2) == 1;
        while (sb.Length < length - digitCount)
        {
            sb.Append(GetRandomChar(vowel));
            if (vowel) vowel = _rnd.Next(6) == 0;
            else vowel = _rnd.Next(6) < 5;
        }
        sb.Append(_rnd.Next(Pow(10, digitCount)));
        return sb.ToString();
    }

    /// <param name="sex">'m' = male, 'f' = female, any other = random</param>
    /// <param name="minAge"></param>
    /// <param name="maxAge"></param>
    /// <param name="mode">Mode of login generation
    /// <list type="table">
    /// <item><description>0 - Random</description></item> 
    /// <item><description>1 - FirstSecond</description></item> 
    /// <item><description>2 - FSecond</description></item> 
    /// <item><description>3 - FirstS</description></item> 
    /// <item><description>4 - SecondFirst</description></item> 
    /// <item><description>5 - SFirst</description></item> 
    /// <item><description>6 - SecondF</description></item> 
    /// <item><description>7 - Random Letter Comnination</description></item> 
    /// </list> 
    /// </param>
    /// <param name="randLetterCombChance">Chance of getting Random letter combination mode [ x/(6+x) ]</param>
    /// <param name="minLoginLength"></param>
    /// <param name="maxLoginLength"></param>
    /// <param name="minRandLetterCombLength">Min length of login if it's a rand letter combination</param>
    /// <param name="maxRandLetterCombLength">Max length of login if it's a rand letter combination</param>
    /// <param name="minPasswordLength"></param>
    /// <param name="maxPasswordLength"></param>
    public Acc(char sex = 'r', int minAge = 19, int maxAge = 40, int mode = 0, int randLetterCombChance = 2, int minLoginLength = 6, int maxLoginLength = 25, int minRandLetterCombLength = 6, int maxRandLetterCombLength = 10, int minPasswordLength = 8, int maxPasswordLength = 16)
    {
        if (maxAge < minAge)
            throw new Exception("Wrong min/max age!");
        if (maxLoginLength < minLoginLength)
            throw new Exception("Wrong min/max login length");
        if (maxPasswordLength < minPasswordLength)
            throw new Exception("Wrong min/max password length");

        RandUpdate();

        if (sex == 'm' || sex == 'f') Sex = sex;
        else Sex = (_rnd.Next(2) == 1) ? 'f' : 'm';
        Name = GetRandomName(Sex);
        SecName = GetRandomName(Sex, true);
        BirthDateTime = GetRandDateTime(minAge, maxAge);
        Login = GetRandomLogin(Name, SecName, mode, randLetterCombChance, minLoginLength, maxLoginLength, minRandLetterCombLength, maxRandLetterCombLength);
        Password = GetRandomLogin(minPasswordLength, maxPasswordLength);
    }

    public string Name { get; set; }
    public string SecName { get; set; }
    public char Sex { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public DateTime BirthDateTime { get; set; }

    public string ToString(string format = "login;password;name;secName;birthDate;sex\r\n", string dateFormat = "dd.MM.yyyy")
    {
        format = format.Replace("login", Login);
        format = format.Replace("password", Password);
        format = format.Replace("name", Name);
        format = format.Replace("secName", SecName);
        format = format.Replace("birthDate", BirthDateTime.ToString(dateFormat));
        format = format.Replace("sex", Sex.ToString());
        return format;
    }

    #region Constatns and static variables

    static Acc()
    {
        RuFemaleNamesCount = new int[2];
        RuMaleNamesCount = new int[2];
        RuMaleNamesCount[0] = RuMaleNames[0].Length;
        RuMaleNamesCount[1] = RuMaleNames[1].Length;
        RuFemaleNamesCount[0] = RuFemaleNames[0].Length;
        RuFemaleNamesCount[1] = RuFemaleNames[1].Length;
        LettersCount = Cyr.Length;
        VowelsCount = Vowels.Length;
        ConsonantsCount = Consonants.Length;
        DividersCount = Dividers.Length;
        _rnd = new ThreadSafeRandom();
    }

    const int RandUpdatePeriod = 4096;
    private static ThreadSafeRandom _rnd;
    private static int _randCallCounter;

    public static int[] RuFemaleNamesCount { get; private set; }
    public static int[] RuMaleNamesCount { get; private set; }
    private static readonly int LettersCount;
    private static readonly int VowelsCount;
    private static readonly int ConsonantsCount;
    private static readonly int DividersCount;

    private static readonly string[][] RuMaleNames =
        {
            new[]
            {
                "Олег", "Эльдар", "Александр", "Андрей", "Ярослав", "Максим", "Ваня", "Иван", "Денис", "Вадим", "Витя",
                "Павел", "Петр", "Рома", "Евгений", "Марк", "Пётр", "Владислав", "Роман", "Сергей", "Игорь", "Станислав",
                "Артемий", "Замин", "Алексей", "Илья", "Константин", "Георгий", "Артем", "Серёга", "Дмитрий", "Макс",
                "Руслан", "Данил", "Никита", "Влад", "Эдик", "Филипп", "Лёша", "Женя", "Даниил", "Всеволод", "Михаил",
                "Кирилл", "Антон", "Виктор", "Святослав", "Николай", "Виталий", "Саша", "Артём", "Борис", "Степан",
                "Паша", "Стас", "Коля", "Рустам", "Артур", "Санёк", "Валера", "Юрий", "Арсений", "Слава", "Юра", "Тимур",
                "Ян", "Богдан", "Генрих", "Дима", "Владимир"
            },
            new[]
            {
                "Кашин", "Романов", "Маргаритов", "Иньков", "Богатырев", "Дмитриев", "Усов", "Потехин", "Олегович", "Бурый",
                "Кувалдин", "Малышев", "Майер", "Стригун", "Павлов", "Чернятин", "Тищенко", "Фильченков", "Беляков",
                "Князев", "Геворкян", "Дубовской", "Скадич", "Григорьев", "Планцов", "Жилис", "Леонтьев", "Давыдов",
                "Сидоров", "Ревизонский", "Корнев", "Башаримов", "Владимирович", "Ремизов", "Король", "Васильев",
                "Калиничев", "Куликов", "Нестеров", "Разумов", "Князьков", "Шихов", "Деянов", "Грицаенко", "Колесов",
                "Волков", "Дацкевич", "Катала", "Марчак", "Комиссаров", "Томилов", "Любимов", "Тушин", "Березняк",
                "Алиев", "Збирко", "Мотов", "Никулин", "Трофимов", "Инин", "Иванов", "Громов", "Каверин", "Турпаков",
                "Польша", "Мерзликин", "Колосов", "Акст", "Щербань", "Подшиблов", "Мамаевский", "Кириллов", "Малахов",
                "Жуков", "Гаврилов", "Коробкин", "Патюлин", "Бабенко", "Сафонов", "Дегтярёв", "Дёмин", "Пашакинскас",
                "Мельников", "Троицкий", "Барышников", "Вишневский", "Пономарюк", "Вавилов", "Марков", "Гамора", "Чайко",
                "Калиберда", "Чернов", "Сергеевич", "Дурасов", "Исаев", "Старинин", "Дубицкий", "Гурьянов", "Якоб",
                "Ворошилин", "Стебаков", "Борисов", "Назаров", "Цуркан", "Ефимов", "Столяров", "Истомин", "Амилевский",
                "Горелов", "Пахомов", "Красс", "Габрусь", "Шумаев", "Орехов", "Сахаров", "Кузовкин", "Калякин", "Канаев",
                "Болячин", "Кузлякин", "Коберман", "Кузнецов", "Решетнёв", "Комин", "Сендецький", "Гафуров", "Акимов",
                "Бубенов", "Андреев", "Касинов", "Дейкин", "Батраков", "Орлов", "Семенов", "Пронин", "Катрук", "Егоров",
                "Дюков", "Дворовой", "Рыжов", "Беспечкин", "Токарчук", "Молотов", "Антонов", "Дорохов", "Мустафаев",
                "Ушаков", "Луговский", "Дадашов", "Ращаускас", "Кулик", "Чудновский", "Матвеев", "Коробков",
                "Чернявский", "Коваленко", "Рафиков", "Буланов", "Червяков", "Штепенко", "Кадочников", "Горенко",
                "Янпольский", "Нестеренко", "Юрченко", "Девятых", "Тамашаускас", "Костюков", "Клименко", "Крюков",
                "Сулимов", "Филатов", "Милевский", "Вилаев", "Дядюченко", "Гордеев", "Пашков", "Горбунов", "Миронов",
                "Овчарик", "Романенко", "Михайлович", "Коробцов", "Кобылкин", "Александрович", "Жилевичус", "Пеев",
                "Шатейка", "Илларионов", "Михайлов", "Костин", "Вещиков", "Карпухин", "Белкин", "Гончаров", "Хрипунков",
                "Харизов", "Рихтер", "Савчуков", "Азаров", "Гореликов", "Волощак", "Кумко", "Кульпин", "Анисимов",
                "Лишик", "Медведев", "Максимов", "Огурец", "Тихомиров", "Фезиев", "Коростылев", "Степанов", "Алёнин",
                "Добрынин", "Белый", "Некрасов", "Митин", "Оборотов", "Буторин", "Божнюк", "Наливкин", "Иващенко",
                "Визовикин", "Морковцев"
            }
        };

    private static readonly string[][] RuFemaleNames =
        {
            new[]
            {
                "Ангелина", "Валентина", "Анастасия", "Лиза", "Валентин", "Оксана", "Любовь", "Алла", "Надежда",
                "Екатерина", "Алена", "Лада", "Форель", "Александра", "Евдокия", "Лилия", "Татьяна", "Капитолина",
                "Софья", "Юлия", "Алёна", "Лола", "Наташа", "Аделина", "Ирина", "Валерия", "Елена", "Полина", "Галина",
                "Мария", "Татяна", "Лерка", "Маргарита", "Анна", "Яраславна", "Алия", "Вера", "Олеся", "Алеся", "Алина",
                "Наталья", "Соня", "Катерина", "Светлана", "Евгения", "Ольга", "Алиса", "Марина", "Даша", "Наида",
                "Своя", "Катя", "Милена", "Анюта", "Юля", "Лаура", "Вика", "Дарина", "Дарья", "Анаит", "Лена", "Ксюша",
                "Люзия", "Семушина", "Снежанна", "Елизавета", "Марианна", "Иринка", "Залина", "Маша", "Людмила", "Сима",
                "Лера", "Марьяна", "Камила", "Лианна", "Мирослава", "Тина", "Кристина", "Алехандра", "Лида", "Марям",
                "Нарисханум", "Аня", "Карина", "Альфия", "Яна", "Нина", "Аделия", "Хайда", "Инна", "Зинаида", "Василиса",
                "Павлина", "Николь", "Магдалена", "Диана", "Ника", "Доша", "Настена", "Марселин", "Всеслава", "Велена",
                "Мафия", "Маринка", "Натали", "Мура", "Эльвира", "Ксения", "Виктория", "Эсмеральда", "Надя", "Галия",
                "Марья", "Анжелика", "Эльмира", "Таня", "Аматулла", "Эльвина"
            },
            new[]
            {
                "Земцова", "Щербакова", "Дёмина", "Мешкова", "Семенова", "Прошунина", "Семёнова", "Климова", "Ковальчук",
                "Филатова", "Емец", "Радужная", "Матросова", "Запасова", "Розенберг", "Ануфриева", "Корнеева",
                "Вячеславовна", "Бурдуковская", "Атаманова", "Потапова", "Дмитриева", "Лобанова", "Майкопова",
                "Малыгина", "Мельник", "Ржевская", "Юрасова", "Поликарпенко", "Пестрикова", "Волкова", "Салищева",
                "Орлова", "Олова", "Лайвли", "Шумкина", "Васильева", "Королева", "Зарифуллина", "Лукьянова", "Коршунова",
                "Жучкова", "Багирова", "Петрова", "Малинковская", "Гамаюнова", "Федосеева", "Лупашку", "Овсянникова",
                "Коженкова", "Ким", "Спасибухова", "Азарова", "Кочетова", "Клипова", "Шукшина", "Григорьева",
                "Некрасова", "Кузнецова", "Котова", "Карпачёва", "Слёзкина", "Дрокина", "Рябинкина", "Квачева",
                "Степанова", "Егреши", "Ворончихина", "Бурмистрова", "Александрова", "Кручинина", "Захарова", "Алева",
                "Жизнь", "Андриенко", "Злодеева", "Бастурма", "Кутузова", "Мишарина", "Гришковец", "Калинина",
                "Магарулай", "Гренева", "Желенова", "Гусеева", "Валиева", "Ильина", "Зайцева", "Дзгоева", "Прудникова",
                "Браун", "Екатерина", "Максимова", "Татьяна", "Александровна", "Татьянина", "Авершина", "Сергеева",
                "Фомина", "Шамонова", "Снегурочка", "Алексеева", "Сотникова", "Ализаде", "Медведева", "Лошкарева",
                "Тяготина", "Белова", "Викина", "Каменева", "Гаджиева", "Бакинка", "Козлова", "Макарова", "Копысова",
                "Бурлак", "Дмитриевна", "Ханова", "Дегтярева", "Мадоян", "Шеллак", "Назарова", "Абубакарова", "Парочка",
                "Семушина", "Чупрова", "Меллер", "Мавроди", "Чадаева", "Колмакова", "Плеханова", "Новицкая", "Ахмедова",
                "Речкина", "Крылова", "Егорченкова", "Герасимова", "Соловьева", "Монтенегро", "Пыжьянова", "Бродова",
                "Коротинаперфильева", "Маркова", "Симонова", "Мотуз", "Соломатина", "Ханян", "Муравьева", "Лапуля",
                "Мамаева", "Бударина", "Лескова", "Милованова", "Симавонова", "Рудская", "Седокова", "Абрамова",
                "Кремер", "Федотова", "Ковалева", "Акмукова", "Суворова", "Богдашктна", "Иконникова", "Лебедева",
                "Романова", "Милькова", "Рахмеева", "Кирина", "Царёва", "Хайда", "Футорянская", "Игоревна", "Скороход",
                "Морозова", "Рязанова", "Калининграда", "Румянцева", "Сергазиева", "Леонтьева", "Владимирова",
                "Никитина", "Гавреленко", "Неважно", "Данова", "Хусенова", "Лернер", "Черёмушкинагалич", "Смирнова",
                "Михайловна", "Няшкина", "Соколова", "Одинокова", "Миронова", "Иванова", "Бобкова", "Цаплина",
                "Безрукова", "Школа", "Тарасова", "Графова", "Церковникова", "Биджамова", "Абадир", "Коновалова",
                "Козырева", "Кондратьева", "Ширялина", "Мафия", "Кунаева", "Щукина", "Елагина", "Савтыра", "Федорова",
                "Мура", "Севостьянова", "Бируж", "Щеголькова", "Цветкова", "Дроздова", "Счастливая", "Токарева",
                "Голонова", "Хомина", "Остапенко", "Маханькова", "Михайлова", "Фролова", "Корякова", "Туркина",
                "Чайкина", "Рябинина", "Багапова", "Андреевна", "Минко", "Родионова", "Шилова", "Алиева", "Холод",
                "Плотникова", "Литвинова", "Дружинова", "Киселёва", "Гарифуллина", "Савельева", "Тихоя", "Роеванская",
                "Мунзафарова", "Вахрушева", "Юнкевич", "Нартова", "Банько", "Мельникова", "Прохорова", "Мамина",
                "Князева", "Просолова", "Гафурова", "Харзинова", "Гуляева", "Мария", "Чибурахова", "Поспелова",
                "Черемных", "Омарова", "Чистякова", "Эдуардовна", "Остапчук"
            }
        };

    private static readonly char[] Cyr =
        {
            'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о',
            'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ы', 'э', 'ю', 'я',
            'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И', 'Й', 'К', 'Л', 'М', 'Н', 'О',
            'П', 'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ы', 'Э', 'Ю', 'Я'
        };

    private static readonly string[] Lat =
        {
            "a", "b", "v", "g", "d", "e", "ye", "zh", "z", "i", "i", "k", "l", "m", "n", "o",
            "p", "r", "s", "t", "u", "f", "kh", "ts", "ch", "sh", "shch", "y", "e", "yu", "ya",
            "A", "B", "V", "G", "D", "E", "YE", "ZH", "Z", "I", "I", "K", "L", "M", "N", "O",
            "P", "R", "S", "T", "U", "F", "KH", "TS", "CH", "SH", "SHCH", "Y", "E", "YU", "YA"
        };

    private static readonly char[] Vowels = { 'a', 'o', 'u', 'e', 'y', 'i' };

    private static readonly char[] Consonants =
        {
            'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q',
            'r', 's', 't', 'v', 'w', 'x', 'z'
        };

    private static readonly string[] Dividers = { "-", ".", "" };
    #endregion
}

/// <summary>
/// Thread safe random value class
/// </summary>
public class ThreadSafeRandom
{
    private static readonly Random Global = new Random();
    [ThreadStatic]
    private static Random _local;

    public ThreadSafeRandom()
    {
        if (_local == null)
        {
            int seed;
            lock (Global)
            {
                seed = Global.Next();
            }
            _local = new Random(seed);
        }
    }
    public int Next()
    {
        return _local.Next();
    }

    public int Next(int maxValue)
    {
        return _local.Next(maxValue);
    }

    public int Next(int minValue, int maxValue)
    {
        return _local.Next(minValue, maxValue);
    }
}