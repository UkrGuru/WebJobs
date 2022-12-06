namespace UkrGuru.WebJobs.Data;

/// <summary>
/// 
/// </summary>
public enum JobStatus
{
    /// <summary>
    /// 
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// 
    /// </summary>
    Queued = 1,
    
    /// <summary>
    /// 
    /// </summary>
    Running = 2,
    
    /// <summary>
    /// 
    /// </summary>
    Completed = 3,
    
    /// <summary>
    /// 
    /// </summary>
    Failed = 4,
    
    /// <summary>
    /// 
    /// </summary>
    Cancelled = 5
}
