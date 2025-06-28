# Coding Guidelines for AI Assistants

## Code Style
- 함수명: camelCase
- 컴포넌트명: PascalCase  
- 상수: UPPER_SNAKE_CASE
- 파일명: kebab-case

## Patterns to Follow
```typescript
// ✅ 선호하는 패턴
const useUserData = (userId: string) => {
  const [user, setUser] = useState<User | null>(null);
  // ... hook logic
  return { user, loading, error };
};

// ✅ 에러 처리 패턴
try {
  const result = await apiCall();
  return { data: result, error: null };
} catch (error) {
  return { data: null, error: error.message };
}
```

## Patterns to Avoid
```typescript
// ❌ 피해야 할 패턴
const userData = useEffect(() => {
  // 직접적인 상태 변경
});

// ❌ 하드코딩된 값들
const API_URL = "http://localhost:3000";
```

## File Organization
```
src/
├── components/     # 재사용 가능한 UI 컴포넌트
├── pages/         # 페이지 컴포넌트
├── hooks/         # 커스텀 훅
├── utils/         # 유틸리티 함수
├── types/         # TypeScript 타입 정의
└── api/           # API 관련 함수
```
