﻿namespace ShoopingWebApplication.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public string UserId { get; set; } 

        public ICollection<CartItem> CartItems { get; set; }
    }
}
