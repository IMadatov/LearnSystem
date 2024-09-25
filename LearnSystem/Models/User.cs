﻿using Microsoft.AspNetCore.Identity;

namespace LearnSystem.Models;

public class User : IdentityUser
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? TelegramId { get; set; }

    public string? PhotoUrl { get; set; }

    public string? Hash { get; set; }

    public string? AuthDate {  get; set; }

    public string? TelegramUserName {  get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

}



/*
 * {
    "id": 629848052,
    "first_name": "ㅤ",
    "last_name": "ㅤㅤ ㅤ И.Мадатов_",
    "username": "islam_madatov",
    "photo_url": "https://t.me/i/userpic/320/xEO7zsVIQ3KTFkSJQgWiMsovTfHqqvN4X_r9y_0rtcs.jpg",
    "auth_date": 1726218416,
    "hash": "6ec2387237c39008002d64ea39333151d32519b13a7a9818247ab23c90b0bd70"
}
 */