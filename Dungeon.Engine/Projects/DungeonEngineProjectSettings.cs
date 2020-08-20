﻿using Dungeon.Data;
using System.ComponentModel.DataAnnotations;

namespace Dungeon.Engine.Projects
{
    public class DungeonEngineProjectSettings
    {
        [Display(Name ="Проброс исключений", Description ="Вместо обработок ошибок приложение будет выбрасывать исключение")]
        public bool ExceptionRethrow { get; set; } = false;

        [Display(Name = "Глобальный перехват Ex", Description = "// означает что глобальный перехват не используется, очистить поле если требуется")]
        public string GlobalExceptionHandling { get; set; } = "//";

        [Display(Name = "Не выгружать ресурсы", Description = "При смене сцены ресурсы не будут выгружаться, это поможет переиспользовать ресурсы, но будет влиять на память")]
        public bool NotDisposingResources { get; set; } = false;

        [Display(Name = "Кэш масок", Description = "Кэширование масок изображений, влияет на производительность")]
        public bool CacheImagesAndMasks { get; set; } = true;
    }
}