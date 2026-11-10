using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Refactor;
using UnityEngine;

namespace Kingdom.Tests
{
    public sealed class CardJsonLoaderTests
    {
        // 한 줄 이동만 가능한 테스트용 카드.
        public const string PawnJson = @"{
            ""id"": ""pawn"",
            ""name"": ""pawn"",
            ""levels"": [ {
                ""level"": 1, ""baseCP"": 10, ""baseHP"": 10,
                ""movement"": { ""type"": ""Line"", ""directions"": ""Orthogonal"", ""maxDistance"": 1 }
            } ]
        }";

        [Test]
        public void LoadsEveryCardFileWithoutError()
        {
            List<CardDefinition> cards =
                CardJsonLoader.LoadDirectory(Path.Combine(Application.streamingAssetsPath, "Cards"));

            Assert.That(cards, Is.Not.Empty);
            // 파일 사이의 중복 ID는 CardDatabase가 검사한다.
            Assert.DoesNotThrow(() => new CardDatabase(cards));
        }

        [Test]
        public void UnknownTypeReportsFileAndPath()
        {
            string json = PawnJson.Replace(@"""Line""", @"""Lien""");

            InvalidDataException exception = Assert.Throws<InvalidDataException>(
                () => CardJsonLoader.Load(new[] { ("bad.json", json) }));

            StringAssert.Contains("bad.json", exception.Message);
            StringAssert.Contains("levels[0].movement", exception.Message);
            StringAssert.Contains("Lien", exception.Message);
        }
    }
}
