﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NTS.Server.Entities
{
    public class StarredNotes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid StarredNotesId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        [ForeignKey(nameof(Note))]
        public Guid NoteId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public Notes? Note { get; set; }
        public ApplicationUsers? User { get; set; }
    }
}
