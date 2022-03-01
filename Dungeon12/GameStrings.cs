﻿using Dungeon.Localization;

namespace Dungeon12.Localization
{
    public class GameStrings : LocalizationStringDictionary<GameStrings>
    {
        public string Warrior { get; set; } = "Воин";
        public string Mage { get; set; } = "Маг";
        public string Thief { get; set; } = "Вор";
        public string Priest { get; set; } = "Священник";

        public string Next { get; set; } = "Далее";

        public string Prev { get; set; } = "Назад";

        public string Cancel { get; set; } = "Отмена";

        public string CreateParty { get; set; } = "Создание отряда";

        public string NewGame { get; set; } = "Новая игра";

        public string Save { get; set; } = "Сохранить";

        public string Load { get; set; } = "Загрузить";

        public string Settings { get; set; } = "Настройки";

        public string Credits { get; set; } = "Авторы";

        public string FastGame { get; set; } = "ККИ";

        public string ExitGame { get; set; } = "Выйти";

        public override string ___RelativeLocalizationFilesPath => "locale";

        public override string ___DefaultLanguageCode => "ru";
    }
}