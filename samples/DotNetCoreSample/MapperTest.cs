﻿using WeihanLi.Common.Helpers;

namespace DotNetCoreSample;

internal class MapperTest
{
    private class MapperA
    {
        public string? Name { get; set; }

        public int Age { get; set; }

        public decimal Money { get; set; }
    }

    private class MapperB
    {
        public string? Name { get; set; }

        public int Age { get; set; }
    }

    public static void Test()
    {
        var mapperA = new MapperA
        {
            Age = 22,
            Name = "Michael",
            Money = 236.66M
        };

        var mapperB = MapHelper.Map<MapperA, MapperB>(mapperA);
        var mapperB1 = MapHelper.MapWith<MapperA, MapperB>(mapperA, "Age");
        var mapperB2 = MapHelper.MapWithout<MapperA, MapperB>(mapperA, "Age");
    }
}
