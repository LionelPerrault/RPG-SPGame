﻿using Dungeon;
using System.ComponentModel.DataAnnotations;

namespace Dungeon12.Entities.Enums
{
    public enum Crafts
    {
        [Display(Name = "Алхимик")]
        [Value("Алхимики занимаются сбором трав и изготовлением зелий. Боевые зелья наносят урон частично поглощающийся защитой от элементов. Не боевые зелья позволяют взаимодействовать с окружением или увеличивать значение характеристик и навыков.")]
        Alchemist, // собирание трав, создание зелий, "физческая магия, смешанный урон (урон частично поглащающийся защитой от элементов)" (химия)

        [Display(Name = "Кузнец")]
        [Value("Кузнецы занимаются добычей руды и полезных ископаемых. После сбора ингридиентов кузнецы могут изготавливать оружие из металлов или с использованием древесных заготовок. Так же кузнецы могут изготавливать и чинить бытовые предметы из металлов.")]
        Blacksmith, // собирание руды, выплавка предметов

        [Display(Name = "Плотник")]
        [Value("Плотники занимаются добычей древесины и смолы. В последствии они могут изготавливать стрелковое оружие из дерева - такое как луки и арбалеты. В мирной жизни плотники изготавливают всевозможную мебель и даже строят деревянные дома.")]
        Carpenter, // собирание древесины, создание предметов

        [Display(Name = "Портной")]
        [Value("Портные занимаются добычей ниток из шерсти и растений, а так же снятием кожи изготовлением из неё предметов одежды.")]
        Tailor, // создание предметов из кожи, ниток (шерсти), и искусственных ниток (изобретения)

        [Display(Name = "Изобретатель")]
        [Value("Изобретатели могут открывать новые рецепты и технологии для других профессий. На основе заготовок они способны создать новый предмет, либо изучить готовые материалы и объяснить их возможные свойства и применение.")]
        Artificer // создание рецептов и предметов, из уже готового
    }
}