namespace OgrenciPortali.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TrackcurrentenrolledusercountinDB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OfferedCourses", "CurrentUserCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OfferedCourses", "CurrentUserCount");
        }
    }
}
