using System.Text.Json.Serialization;

namespace BackEnd.Models.MetaSchools;

/// <summary>
/// MetaSchools student with recursive learning tracking
/// </summary>
public record MetaSchoolStudent
{
    [JsonPropertyName("studentId")]
    public string StudentId { get; init; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("ageGroup")]
    public AgeGroup AgeGroup { get; init; }
    
    [JsonPropertyName("currentLevel")]
    public LearningLevel CurrentLevel { get; init; }
    
    [JsonPropertyName("masteryLevel")]
    public double MasteryLevel { get; init; }
    
    [JsonPropertyName("teachingScore")]
    public double TeachingScore { get; init; }
    
    [JsonPropertyName("studentsTeaching")]
    public List<string> StudentsTeaching { get; init; } = new();
    
    [JsonPropertyName("enrollmentDate")]
    public DateTime EnrollmentDate { get; init; }
    
    [JsonPropertyName("progressPath")]
    public List<string> ProgressPath { get; init; } = new();
}

public enum AgeGroup
{
    Seeds,         // 3-5 years
    Sprouts,       // 6-8 years
    Branches,      // 9-11 years
    Trunks,        // 12-14 years
    Forests        // 15-18 years
}

public enum LearningLevel
{
    Learn,
    Apply,
    Teach,
    Innovate
}

/// <summary>
/// MetaSchool curriculum module
/// </summary>
public record CurriculumModule
{
    [JsonPropertyName("moduleId")]
    public string ModuleId { get; init; } = string.Empty;
    
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;
    
    [JsonPropertyName("ageGroup")]
    public AgeGroup AgeGroup { get; init; }
    
    [JsonPropertyName("subject")]
    public string Subject { get; init; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;
    
    [JsonPropertyName("evolverseIntegration")]
    public List<string> EvolverseIntegration { get; init; } = new();
    
    [JsonPropertyName("recursiveDepth")]
    public int RecursiveDepth { get; init; }
}

/// <summary>
/// Teaching session record for recursive education
/// </summary>
public record TeachingSession
{
    [JsonPropertyName("sessionId")]
    public string SessionId { get; init; } = string.Empty;
    
    [JsonPropertyName("teacherId")]
    public string TeacherId { get; init; } = string.Empty;
    
    [JsonPropertyName("studentIds")]
    public List<string> StudentIds { get; init; } = new();
    
    [JsonPropertyName("moduleId")]
    public string ModuleId { get; init; } = string.Empty;
    
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; }
    
    [JsonPropertyName("effectiveness")]
    public double Effectiveness { get; init; }
    
    [JsonPropertyName("recursiveGeneration")]
    public int RecursiveGeneration { get; init; }
}

/// <summary>
/// MetaSchools statistics and analytics
/// </summary>
public record MetaSchoolsStatistics
{
    [JsonPropertyName("totalStudents")]
    public int TotalStudents { get; init; }
    
    [JsonPropertyName("studentsByAgeGroup")]
    public Dictionary<AgeGroup, int> StudentsByAgeGroup { get; init; } = new();
    
    [JsonPropertyName("averageMasteryLevel")]
    public double AverageMasteryLevel { get; init; }
    
    [JsonPropertyName("totalTeachingSessions")]
    public int TotalTeachingSessions { get; init; }
    
    [JsonPropertyName("recursiveDepth")]
    public int RecursiveDepth { get; init; }
    
    [JsonPropertyName("studentsTeaching")]
    public int StudentsTeaching { get; init; }
}

/// <summary>
/// Student progression report
/// </summary>
public record StudentProgressionReport
{
    [JsonPropertyName("studentId")]
    public string StudentId { get; init; } = string.Empty;
    
    [JsonPropertyName("currentLevel")]
    public LearningLevel CurrentLevel { get; init; }
    
    [JsonPropertyName("masteryProgress")]
    public double MasteryProgress { get; init; }
    
    [JsonPropertyName("teachingImpact")]
    public double TeachingImpact { get; init; }
    
    [JsonPropertyName("readyForPromotion")]
    public bool ReadyForPromotion { get; init; }
    
    [JsonPropertyName("nextMilestone")]
    public string NextMilestone { get; init; } = string.Empty;
}
