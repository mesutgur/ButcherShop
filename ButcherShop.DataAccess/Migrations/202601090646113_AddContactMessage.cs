namespace ButcherShop.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddContactMessage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContactMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Subject = c.String(),
                        Message = c.String(),
                        IsRead = c.Boolean(nullable: false),
                        ReadDate = c.DateTime(),
                        AdminNote = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ContactMessages");
        }
    }
}
