using BackEnd.Models.MetaSchools;

namespace BackEnd.Services.MetaSchools;

/// <summary>
/// MetaSchools Service - Kid-first recursive education framework
/// Every student becomes a teacher, creating recursive knowledge transfer
/// </summary>
public class MetaSchoolsService
{
    private readonly List<MetaSchoolStudent> _students = new();
    private readonly List<CurriculumModule> _curriculum = new();
    private readonly List<TeachingSession> _sessions = new();
    private readonly ILogger<MetaSchoolsService> _logger;
    private int _studentCounter = 1;
    private int _sessionCounter = 1;

    public MetaSchoolsService(ILogger<MetaSchoolsService> logger)
    {
        _logger = logger;
        InitializeCurriculum();
    }

    /// <summary>
    /// Initialize MetaSchools curriculum with EV0LVerse integration
    /// </summary>
    private void InitializeCurriculum()
    {
        _curriculum.AddRange(new[]
        {
            new CurriculumModule
            {
                ModuleId = "MOD-001",
                Title = "ES0IL Garden Discovery",
                AgeGroup = AgeGroup.Seeds,
                Subject = "Nature & Agriculture",
                Description = "Hands-on learning with ES0IL substrate, growing food and observing life cycles",
                EvolverseIntegration = new List<string> { "ES0IL", "Agriculture" },
                RecursiveDepth = 0
            },
            new CurriculumModule
            {
                ModuleId = "MOD-002",
                Title = "BLEU Flame Economics",
                AgeGroup = AgeGroup.Branches,
                Subject = "Economics & Technology",
                Description = "Understanding token economics, MetaVault yield, and cross-sector income",
                EvolverseIntegration = new List<string> { "BLEU_FLAME", "METAVAULT" },
                RecursiveDepth = 1
            },
            new CurriculumModule
            {
                ModuleId = "MOD-003",
                Title = "Zion Gold Bar & Resource Classification",
                AgeGroup = AgeGroup.Trunks,
                Subject = "Economics & History",
                Description = "Saturn-Strata classification, wealth narratives, and resource economics",
                EvolverseIntegration = new List<string> { "ZION_GOLD_BAR" },
                RecursiveDepth = 1
            },
            new CurriculumModule
            {
                ModuleId = "MOD-004",
                Title = "Recursive Systems Design",
                AgeGroup = AgeGroup.Forests,
                Subject = "Computer Science & Philosophy",
                Description = "Creating self-teaching systems, recursive algorithms, and knowledge transfer",
                EvolverseIntegration = new List<string> { "CODEX", "OVERSCALE" },
                RecursiveDepth = 2
            }
        });

        // Module counter starts after initialized curriculum modules
    }

    /// <summary>
    /// Enroll a new student
    /// </summary>
    public Task<MetaSchoolStudent> EnrollStudent(string name, AgeGroup ageGroup)
    {
        var studentId = $"STUDENT-{_studentCounter:D6}";
        _studentCounter++;

        var student = new MetaSchoolStudent
        {
            StudentId = studentId,
            Name = name,
            AgeGroup = ageGroup,
            CurrentLevel = LearningLevel.Learn,
            MasteryLevel = 0.0,
            TeachingScore = 0.0,
            EnrollmentDate = DateTime.UtcNow,
            ProgressPath = new List<string> { "Enrolled" }
        };

        _students.Add(student);
        _logger.LogInformation($"Enrolled student {studentId}: {name} in {ageGroup}");

        return Task.FromResult(student);
    }

    /// <summary>
    /// Progress a student to the next learning level
    /// </summary>
    public Task<MetaSchoolStudent> ProgressStudent(string studentId, double newMasteryLevel)
    {
        var student = _students.FirstOrDefault(s => s.StudentId == studentId);
        if (student == null)
            throw new KeyNotFoundException($"Student {studentId} not found");

        var newLevel = newMasteryLevel >= 0.8 ? LearningLevel.Teach :
                       newMasteryLevel >= 0.6 ? LearningLevel.Apply :
                       LearningLevel.Learn;

        var progressedStudent = student with 
        { 
            MasteryLevel = newMasteryLevel,
            CurrentLevel = newLevel,
            ProgressPath = student.ProgressPath.Concat(new[] { $"Progressed to {newLevel}" }).ToList()
        };

        var index = _students.FindIndex(s => s.StudentId == studentId);
        _students[index] = progressedStudent;

        _logger.LogInformation($"Progressed student {studentId} to {newLevel} with mastery {newMasteryLevel:P0}");

        return Task.FromResult(progressedStudent);
    }

    /// <summary>
    /// Record a teaching session (recursive education)
    /// </summary>
    public Task<TeachingSession> RecordTeachingSession(
        string teacherId, 
        List<string> studentIds, 
        string moduleId,
        double effectiveness)
    {
        var sessionId = $"SESSION-{_sessionCounter:D8}";
        _sessionCounter++;

        var teacher = _students.FirstOrDefault(s => s.StudentId == teacherId);
        if (teacher == null)
            throw new KeyNotFoundException($"Teacher {teacherId} not found");

        // Calculate recursive generation (how many levels deep in teaching chain)
        var generation = CalculateRecursiveGeneration(teacherId);

        var session = new TeachingSession
        {
            SessionId = sessionId,
            TeacherId = teacherId,
            StudentIds = studentIds,
            ModuleId = moduleId,
            Timestamp = DateTime.UtcNow,
            Effectiveness = effectiveness,
            RecursiveGeneration = generation
        };

        _sessions.Add(session);

        // Update teacher's teaching score and students list
        var updatedTeacher = teacher with
        {
            TeachingScore = teacher.TeachingScore + effectiveness,
            StudentsTeaching = teacher.StudentsTeaching.Concat(studentIds).Distinct().ToList()
        };

        var teacherIndex = _students.FindIndex(s => s.StudentId == teacherId);
        _students[teacherIndex] = updatedTeacher;

        _logger.LogInformation($"Recorded teaching session {sessionId} by {teacherId} at generation {generation}");

        return Task.FromResult(session);
    }

    /// <summary>
    /// Get all students
    /// </summary>
    public Task<List<MetaSchoolStudent>> GetAllStudents()
    {
        return Task.FromResult(_students.ToList());
    }

    /// <summary>
    /// Get student by ID
    /// </summary>
    public Task<MetaSchoolStudent?> GetStudent(string studentId)
    {
        return Task.FromResult(_students.FirstOrDefault(s => s.StudentId == studentId));
    }

    /// <summary>
    /// Get students by age group
    /// </summary>
    public Task<List<MetaSchoolStudent>> GetStudentsByAgeGroup(AgeGroup ageGroup)
    {
        return Task.FromResult(_students.Where(s => s.AgeGroup == ageGroup).ToList());
    }

    /// <summary>
    /// Get all curriculum modules
    /// </summary>
    public Task<List<CurriculumModule>> GetCurriculum()
    {
        return Task.FromResult(_curriculum.ToList());
    }

    /// <summary>
    /// Get curriculum for specific age group
    /// </summary>
    public Task<List<CurriculumModule>> GetCurriculumForAgeGroup(AgeGroup ageGroup)
    {
        return Task.FromResult(_curriculum.Where(m => m.AgeGroup == ageGroup).ToList());
    }

    /// <summary>
    /// Get MetaSchools statistics
    /// </summary>
    public Task<MetaSchoolsStatistics> GetStatistics()
    {
        var stats = new MetaSchoolsStatistics
        {
            TotalStudents = _students.Count,
            StudentsByAgeGroup = _students.GroupBy(s => s.AgeGroup)
                .ToDictionary(g => g.Key, g => g.Count()),
            AverageMasteryLevel = _students.Any() ? _students.Average(s => s.MasteryLevel) : 0,
            TotalTeachingSessions = _sessions.Count,
            RecursiveDepth = _sessions.Any() ? _sessions.Max(s => s.RecursiveGeneration) + 1 : 0,
            StudentsTeaching = _students.Count(s => s.StudentsTeaching.Any())
        };

        return Task.FromResult(stats);
    }

    /// <summary>
    /// Get student progression report
    /// </summary>
    public Task<StudentProgressionReport> GetProgressionReport(string studentId)
    {
        var student = _students.FirstOrDefault(s => s.StudentId == studentId);
        if (student == null)
            throw new KeyNotFoundException($"Student {studentId} not found");

        var readyForPromotion = student.MasteryLevel >= 0.8 && student.CurrentLevel != LearningLevel.Innovate;
        var nextMilestone = GetNextMilestone(student);

        var report = new StudentProgressionReport
        {
            StudentId = studentId,
            CurrentLevel = student.CurrentLevel,
            MasteryProgress = student.MasteryLevel,
            TeachingImpact = student.StudentsTeaching.Count * student.TeachingScore,
            ReadyForPromotion = readyForPromotion,
            NextMilestone = nextMilestone
        };

        return Task.FromResult(report);
    }

    /// <summary>
    /// Calculate recursive generation for a teacher (how deep in teaching chain)
    /// </summary>
    private int CalculateRecursiveGeneration(string teacherId)
    {
        var generation = 0;
        var currentId = teacherId;
        var visited = new HashSet<string>();

        while (currentId != null && !visited.Contains(currentId))
        {
            visited.Add(currentId);
            var taughtBy = _sessions.FirstOrDefault(s => s.StudentIds.Contains(currentId));
            if (taughtBy != null)
            {
                generation++;
                currentId = taughtBy.TeacherId;
            }
            else
            {
                break;
            }
        }

        return generation;
    }

    /// <summary>
    /// Get next milestone for student
    /// </summary>
    private string GetNextMilestone(MetaSchoolStudent student)
    {
        return student.CurrentLevel switch
        {
            LearningLevel.Learn => $"Reach 60% mastery to Apply knowledge",
            LearningLevel.Apply => $"Reach 80% mastery to begin Teaching",
            LearningLevel.Teach => $"Teach {5 - student.StudentsTeaching.Count} more students to Innovate",
            LearningLevel.Innovate => "Create new curriculum or system",
            _ => "Continue learning"
        };
    }
}
