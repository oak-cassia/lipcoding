# 데이터베이스 설계 문서

## 📋 데이터베이스 개요

### 기술 스택
- **Database**: SQLite (로컬 개발)
- **ORM**: Entity Framework Core 8.0
- **Migration**: EF Core Code-First 방식

### 설계 원칙
- 정규화된 테이블 구조
- 외래키 제약조건 적용
- 인덱스 최적화
- 데이터 무결성 보장

---

## 🗄️ 테이블 설계

### 1. Users 테이블

```sql
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    Name NVARCHAR(100),
    Role NVARCHAR(10) NOT NULL CHECK (Role IN ('Mentor', 'Mentee')),
    Bio TEXT,
    ProfileImage BLOB,
    Skills TEXT, -- JSON 형태로 저장
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 인덱스
CREATE UNIQUE INDEX IX_Users_Email ON Users (Email);
CREATE INDEX IX_Users_Role ON Users (Role);
CREATE INDEX IX_Users_CreatedAt ON Users (CreatedAt);
```

#### 컬럼 설명
- `Id`: 사용자 고유 식별자 (Primary Key)
- `Email`: 로그인용 이메일 (Unique)
- `PasswordHash`: BCrypt 해시된 비밀번호
- `Name`: 사용자 실명
- `Role`: 사용자 역할 (Mentor/Mentee)
- `Bio`: 자기소개
- `ProfileImage`: 프로필 이미지 (BLOB)
- `Skills`: 기술 스택 (JSON 배열, 멘토만)
- `CreatedAt`: 생성일시
- `UpdatedAt`: 수정일시

### 2. MatchRequests 테이블

```sql
CREATE TABLE MatchRequests (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    MentorId INTEGER NOT NULL,
    MenteeId INTEGER NOT NULL,
    Message TEXT,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending' 
        CHECK (Status IN ('Pending', 'Accepted', 'Rejected', 'Cancelled')),
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (MentorId) REFERENCES Users(Id) ON DELETE RESTRICT,
    FOREIGN KEY (MenteeId) REFERENCES Users(Id) ON DELETE RESTRICT
);

-- 인덱스
CREATE UNIQUE INDEX IX_MatchRequests_MentorId_MenteeId ON MatchRequests (MentorId, MenteeId);
CREATE INDEX IX_MatchRequests_MentorId ON MatchRequests (MentorId);
CREATE INDEX IX_MatchRequests_MenteeId ON MatchRequests (MenteeId);
CREATE INDEX IX_MatchRequests_Status ON MatchRequests (Status);
CREATE INDEX IX_MatchRequests_CreatedAt ON MatchRequests (CreatedAt);
```

#### 컬럼 설명
- `Id`: 매칭 요청 고유 식별자 (Primary Key)
- `MentorId`: 멘토 사용자 ID (Foreign Key)
- `MenteeId`: 멘티 사용자 ID (Foreign Key)
- `Message`: 요청 메시지
- `Status`: 요청 상태 (Pending/Accepted/Rejected/Cancelled)
- `CreatedAt`: 생성일시
- `UpdatedAt`: 수정일시

#### 제약조건
- `(MentorId, MenteeId)`: 복합 유니크 키 (중복 요청 방지)
- `MentorId`, `MenteeId`: 외래키 제약조건 (DELETE RESTRICT)

---

## 📊 Entity Framework Core 모델

### User Entity

```csharp
public class User
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Name { get; set; }
    
    [Required]
    public UserRole Role { get; set; }
    
    public string? Bio { get; set; }
    
    public byte[]? ProfileImage { get; set; }
    
    public string? Skills { get; set; } // JSON 문자열
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public virtual ICollection<MatchRequest> SentRequests { get; set; } = new List<MatchRequest>();
    public virtual ICollection<MatchRequest> ReceivedRequests { get; set; } = new List<MatchRequest>();
}

public enum UserRole
{
    Mentor,
    Mentee
}
```

### MatchRequest Entity

```csharp
public class MatchRequest
{
    public int Id { get; set; }
    
    [Required]
    public int MentorId { get; set; }
    
    [Required]
    public int MenteeId { get; set; }
    
    public string? Message { get; set; }
    
    [Required]
    public MatchRequestStatus Status { get; set; } = MatchRequestStatus.Pending;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    [ForeignKey(nameof(MentorId))]
    public virtual User Mentor { get; set; } = null!;
    
    [ForeignKey(nameof(MenteeId))]
    public virtual User Mentee { get; set; } = null!;
}

public enum MatchRequestStatus
{
    Pending,
    Accepted,
    Rejected,
    Cancelled
}
```

### DbContext 설정

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<MatchRequest> MatchRequests { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User 설정
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");
                
            entity.HasIndex(e => e.Role)
                .HasDatabaseName("IX_Users_Role");
                
            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_Users_CreatedAt");
            
            entity.Property(e => e.Role)
                .HasConversion<string>()
                .HasMaxLength(10);
                
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.Name)
                .HasMaxLength(100);
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        
        // MatchRequest 설정
        modelBuilder.Entity<MatchRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // 복합 유니크 인덱스 (중복 요청 방지)
            entity.HasIndex(e => new { e.MentorId, e.MenteeId })
                .IsUnique()
                .HasDatabaseName("IX_MatchRequests_MentorId_MenteeId");
                
            entity.HasIndex(e => e.MentorId)
                .HasDatabaseName("IX_MatchRequests_MentorId");
                
            entity.HasIndex(e => e.MenteeId)
                .HasDatabaseName("IX_MatchRequests_MenteeId");
                
            entity.HasIndex(e => e.Status)
                .HasDatabaseName("IX_MatchRequests_Status");
                
            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_MatchRequests_CreatedAt");
            
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // 외래키 관계 설정
            entity.HasOne(e => e.Mentor)
                .WithMany(u => u.ReceivedRequests)
                .HasForeignKey(e => e.MentorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_MatchRequests_Mentor");
                
            entity.HasOne(e => e.Mentee)
                .WithMany(u => u.SentRequests)
                .HasForeignKey(e => e.MenteeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_MatchRequests_Mentee");
        });
    }
    
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    private void UpdateTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            
        foreach (var entity in entities)
        {
            if (entity.State == EntityState.Added)
            {
                if (entity.Property("CreatedAt") != null)
                    entity.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
            }
            
            if (entity.Property("UpdatedAt") != null)
                entity.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
        }
    }
}
```

---

## 🔍 쿼리 최적화

### 자주 사용되는 쿼리 패턴

#### 1. 멘토 목록 조회 (기술 스택 필터링)
```csharp
// 기술 스택으로 필터링
var mentors = await context.Users
    .Where(u => u.Role == UserRole.Mentor)
    .Where(u => u.Skills != null && u.Skills.Contains(skillName))
    .OrderBy(u => u.Name) // 또는 Skills로 정렬
    .ToListAsync();
```

#### 2. 매칭 요청 조회 (멘토 기준)
```csharp
// 멘토가 받은 요청 목록
var incomingRequests = await context.MatchRequests
    .Include(mr => mr.Mentee)
    .Where(mr => mr.MentorId == mentorId)
    .OrderByDescending(mr => mr.CreatedAt)
    .ToListAsync();
```

#### 3. 매칭 요청 조회 (멘티 기준)
```csharp
// 멘티가 보낸 요청 목록
var outgoingRequests = await context.MatchRequests
    .Include(mr => mr.Mentor)
    .Where(mr => mr.MenteeId == menteeId)
    .OrderByDescending(mr => mr.CreatedAt)
    .ToListAsync();
```

#### 4. 중복 요청 확인
```csharp
// 이미 요청했는지 확인
var existingRequest = await context.MatchRequests
    .FirstOrDefaultAsync(mr => 
        mr.MentorId == mentorId && 
        mr.MenteeId == menteeId);
```

---

## 📦 초기 데이터 (Seed Data)

### 테스트용 사용자 생성

```csharp
public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync())
            return; // 이미 데이터가 있으면 실행하지 않음
        
        // 테스트 멘토 생성
        var mentors = new[]
        {
            new User
            {
                Email = "mentor1@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Name = "김프론트",
                Role = UserRole.Mentor,
                Bio = "React, Vue.js 전문 멘토입니다.",
                Skills = JsonSerializer.Serialize(new[] { "React", "Vue", "JavaScript", "TypeScript" })
            },
            new User
            {
                Email = "mentor2@example.com", 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Name = "이백엔드",
                Role = UserRole.Mentor,
                Bio = "Spring Boot, Node.js 백엔드 전문가입니다.",
                Skills = JsonSerializer.Serialize(new[] { "Spring Boot", "Node.js", "Java", "Python" })
            }
        };
        
        // 테스트 멘티 생성
        var mentees = new[]
        {
            new User
            {
                Email = "mentee1@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Name = "박학습",
                Role = UserRole.Mentee,
                Bio = "프론트엔드 개발을 배우고 싶습니다."
            },
            new User
            {
                Email = "mentee2@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Name = "최성장",
                Role = UserRole.Mentee,
                Bio = "백엔드 개발자가 되고 싶어요."
            }
        };
        
        context.Users.AddRange(mentors);
        context.Users.AddRange(mentees);
        await context.SaveChangesAsync();
    }
}
```

---

## 🔧 마이그레이션 관리

### 초기 마이그레이션 생성
```bash
dotnet ef migrations add InitialCreate --project src/MentorMenteeApp.API
```

### 데이터베이스 업데이트
```bash
dotnet ef database update --project src/MentorMenteeApp.API
```

### 마이그레이션 히스토리
- `20250628_InitialCreate`: 초기 테이블 생성
- `20250628_AddIndexes`: 인덱스 최적화
- `20250628_SeedData`: 초기 데이터 추가

---

## 🛡️ 데이터 무결성 및 제약조건

### 비즈니스 규칙
1. **중복 요청 방지**: (MentorId, MenteeId) 복합 유니크 키
2. **역할 제한**: 멘토는 ReceivedRequests만, 멘티는 SentRequests만
3. **상태 전이**: Pending → Accepted/Rejected/Cancelled
4. **외래키 무결성**: 사용자 삭제 시 관련 요청 삭제 제한

### 성능 최적화
1. **인덱스 전략**: 자주 조회되는 컬럼에 인덱스 생성
2. **페이징**: 대량 데이터 조회 시 페이징 적용
3. **지연 로딩**: Navigation Property 필요시에만 로드
4. **캐싱**: 자주 조회되는 데이터 메모리 캐싱
