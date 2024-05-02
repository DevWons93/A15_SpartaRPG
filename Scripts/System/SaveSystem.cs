﻿
using Newtonsoft.Json;

namespace TextRPG
{

    struct PlayerDatas // Json에 저장할 플레이어 데이터 구조체
    { 
        public Player player;
        public List<EquipItem> playerItems;
        public List<EquipItem> shopItems;

        public PlayerDatas()
        {
            player = GameManager.instance.Player;
            playerItems = ItemDataManager.instance.PlayerEquipItems;
            shopItems = ItemDataManager.instance.ShopEquipItems;
        }
    }

    public class SaveSystem
    {
        // Exe 파일이 들어 있는 경로 저장
        private string path = AppDomain.CurrentDomain.BaseDirectory;

        public void Save()
        {
            string filePath = $"{path}\\{GameManager.instance.Player.Name}.json";

            // PlayerDatas 구조체 생성
            PlayerDatas data = new PlayerDatas(); 

            // 저장된 플레이어의 정보를 직렬화
            string jsonData = JsonConvert.SerializeObject(data);

            // 플레이어 데이터 Json 파일 저장
            using (var js = new StreamWriter(filePath))
            {
                js.WriteLine(jsonData.ToString());
                js.Close();
            }
        }

        public Player Load(string name)
        {
            string filePath = $"{path}\\{name}.json";
            string equipitemJsonFilePath = Path.Combine(path, @"..\..\..\SaveData\EquipItemData.json");

            // 전체 아이템 데이터 불러오기
            string equipItemJsonData = File.ReadAllText(equipitemJsonFilePath); 
            ItemDataManager.instance.SetItemDB(JsonConvert.DeserializeObject<List<EquipItem>>(equipItemJsonData));

            if (File.Exists(filePath)) // 파일이 존재하는지 -> 이미 데이터가 존재하는 지
            {
                // 데이터가 존재한다면 해당 데이터를 읽어옴
                string playerJsonData = File.ReadAllText(filePath);

                // PlayerDatas 구조체로 역직렬화해서 텍스트를 데이터로 변환
                PlayerDatas playerData = JsonConvert.DeserializeObject<PlayerDatas>(playerJsonData);

                // 불러온 플레이어 정보 저장
                ItemDataManager.instance.SetItems(playerData.shopItems, playerData.playerItems);
                return playerData.player;

            }
            else // 데이터가 없다면
            {
                // 기본 데이터로 초기화
                ItemDataManager.instance.Init();
                return new Player(name);
            }
        }

    }
}
