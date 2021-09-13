﻿using System.ComponentModel.DataAnnotations;

namespace SharedKernel.Enums
{
    public enum SubscribedProducts
    {
        //These are tenant-specific (not user)
        MVConnect,
        InSpec,
        TireTools,
        [Display(Name = "Text Messaging")]
        TextMessaging
    }
}
