﻿namespace LearnSystem.Models.ModelsDTO;

public class ChangeRoleUserDto
{
    public bool AccountStatus { get; set; }
    public string Role { get; set; }
    public Guid UserId {  get; set; }
}
