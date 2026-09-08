using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Oficios.Entities.MicrosoftEntity;

namespace Oficios.DataAccess.MicrosoftIdentity
{
    public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
    {
        public void Configure(EntityTypeBuilder<UserLogin> builder)

        {
            builder.ToTable(nameof(UserLogin));
        }
    }
}

