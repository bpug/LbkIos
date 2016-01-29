using System;
using SQLite;

namespace Loewenbraeu.Model
{
	public abstract class BaseEntity
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public virtual bool Deleted{ get; set; }
		public virtual DateTime CreateAt{ get; set; }
		public virtual DateTime ModifyAt {get; set; }
		
	}
}

