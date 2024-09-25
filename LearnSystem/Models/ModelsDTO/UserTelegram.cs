namespace LearnSystem.Models.ModelsDTO
{
    public class UserTelegram
    {
        public string id { get; set; }

        public string? first_name { get; set; }

        public string? username { get; set; }

        public string? last_name { get; set; }

        public string? photo_url { get; set; }

        public string auth_date { get; set; }

        public string hash { get; set; }

    }
}
/*
 
    "id": 629848052,
    "first_name": "ㅤ",
    "last_name": "ㅤㅤ ㅤ И.Мадатов_",
    "username": "islam_madatov",
    "photo_url": "https://t.me/i/userpic/320/xEO7zsVIQ3KTFkSJQgWiMsovTfHqqvN4X_r9y_0rtcs.jpg",
    "auth_date": 1726230874,
    "hash": "d6c38e4755f00d5fb294074883ac0fc1361555abe4e8cd8d619fb2e3486fb27e"
}
 */