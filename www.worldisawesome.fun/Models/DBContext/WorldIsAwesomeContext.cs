using Microsoft.EntityFrameworkCore;
using www.worldisawesome.fun.DataModels;

namespace www.worldisawesome.fun.DBContext
{
    public partial class WorldIsAwesomeContext : DbContext
    {
        public WorldIsAwesomeContext()
        {
        }

        public WorldIsAwesomeContext(DbContextOptions<WorldIsAwesomeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Experiences> Experiences { get; set; }
        public virtual DbSet<Files> Files { get; set; }
        public virtual DbSet<Places> Places { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Users_ExperiencesToDo> Users_ExperiencesToDo { get; set; }
        public virtual DbSet<Users_UsersFriends> Users_UsersFriends { get; set; }

        #region Views
        public virtual DbSet<View_PlaceExperiences> View_PlaceExperiences { get; set; }
        public virtual DbSet<View_ExperienceFiles> View_ExperienceFiles { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /*if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=WorldIsAwesome;Trusted_Connection=True;");
            }*/
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Experiences>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    //.IsRequired()
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("nvarchar(100)")
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Time).HasColumnType("time");

                entity.HasOne(d => d.MediaFile)
                    .WithMany(p => p.Experiences)
                    .HasForeignKey(d => d.MediaFileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Experiences_Files");

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.Experiences)
                    .HasForeignKey(d => d.PlaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Experiences_Places");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Experiences)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Experiences_Users");

                entity.Property(e => e.DateTimeCreated).HasColumnType("datetimeoffset");

                entity.Property(e => e.StatusEnum).IsRequired();
            });

            modelBuilder.Entity<Files>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FileType)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FileCreated).HasColumnType("datetimeoffset");
            });

            modelBuilder.Entity<Places>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("nvarchar(max)");

                entity.Property(e => e.Latitude).HasColumnType("decimal(10, 8)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(11, 8)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("nvarchar(100)")
                    .IsUnicode(false);

                entity.HasOne(d => d.ProfilePictureFile)
                    .WithMany(p => p.Places)
                    .HasForeignKey(d => d.ProfilePictureFileId)
                    .HasConstraintName("FK_Places_Files");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BirthDate).HasColumnType("date");

                entity.Property(e => e.Nickname)
                    .IsRequired()
                    .HasColumnType("nvarchar(100)")
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.Mail)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.ProfilePhotoFile)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.ProfilePhotoFileId)
                    .HasConstraintName("FK_Users_Files");

                entity.HasOne(d => d.ResidencePlace)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.ResidencePlaceId)
                    .HasConstraintName("FK_Users_Places");

                entity.HasIndex(e => e.Nickname)
                    .IsUnique();

                entity.HasIndex(e => e.Mail)
                    .IsUnique();

                entity.Property(e => e.RegistrationDateTime).HasColumnType("datetimeoffset");
            });

            modelBuilder.Entity<Users_ExperiencesToDo>(entity =>
            {
                //entity.Property(e => e.UserId);
                //entity.Property(e => e.ExperienceId);

                entity.HasKey(ue => new { ue.UserId, ue.ExperienceId });

                entity
                    .HasOne(ue => ue.User)
                    .WithMany(u => u.ExperiencesToDo)
                    .HasForeignKey(ue => ue.UserId);

                entity
                    .HasOne(ue => ue.ExperienceToDo)
                    .WithMany(e => e.UsersToDo)
                    .HasForeignKey(ue => ue.ExperienceId);
            });

            modelBuilder.Entity<Users_UsersFriends>(entity =>
            {
                //entity.Property(e => e.UserId);
                //entity.Property(e => e.UserFriendId);

                entity.HasKey(uu => new { uu.UserId, uu.UserFriendId });

                entity
                    .HasOne(uu => uu.User)
                    .WithMany(u => u.UsersFriends)
                    .HasForeignKey(uu => uu.UserId);
                /*
                entity
                    .HasOne(uu => uu.UserFriend)
                    .WithMany(u => u.UsersFriends)
                    .HasForeignKey(uu => uu.UserFriendId);
                */

                entity.Property(uu => uu.StatusEnum)
                    .IsRequired();
                entity.Property(uu => uu.RequestedDateTime)
                    .IsRequired()
                    .HasColumnType("datetimeoffset");
                entity.Property(uu => uu.AcceptedDateTime)
                    .HasColumnType("datetimeoffset");
            });


            #region VIEWS
            modelBuilder.Entity<View_PlaceExperiences>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.PlaceLatitude).HasColumnType("decimal(10, 8)");
                entity.Property(e => e.PlaceLongitude).HasColumnType("decimal(11, 8)");
            });
            modelBuilder.Entity<View_ExperienceFiles>().HasNoKey();
            #endregion

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
