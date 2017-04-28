namespace WebAssembly.Instructions
{
	/// <summary>
	/// Round to nearest integer towards zero.
	/// </summary>
	public class Float32Truncate : Instruction
	{
		/// <summary>
		/// Always <see cref="OpCode.Float32Truncate"/>.
		/// </summary>
		public sealed override OpCode OpCode => OpCode.Float32Truncate;

		/// <summary>
		/// Creates a new  <see cref="Float32Truncate"/> instance.
		/// </summary>
		public Float32Truncate()
		{
		}
	}
}