// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace UkrGuru.WebJobs.Data;

public partial class ActionInput 
{
    [Display(Name = "Id")]
    public int ActionId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Action")]
    public string ActionName { get; set; }

    [Required]
    [StringLength(255)]
    [Display(Name = "Type")]
    public string ActionType { get; set; }

    [Display(Name = "More")]
    public string ActionMore { get; set; }
}
