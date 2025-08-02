# 공격 애니메이션 분리 설정 가이드

## 📁 **새로운 구조**

### **AttackAnimationController.cs** (`Assets/Scripts/Object/Token/`)
- 공격 애니메이션 전용 스크립트
- 근접 공격 (칼 휘두르기) 및 원거리 공격 (활과 화살) 애니메이션 담당
- 테스트 기능 포함

### **TokenMovement.cs**
- 기존 이동 관련 기능만 유지
- `AttackAnimationController`를 참조하여 공격 애니메이션 실행

## 🔧 **설정 방법**

### **1. AttackAnimationController 설정**
1. 토큰 GameObject에 `AttackAnimationController` 컴포넌트 추가
2. Inspector에서 다음 항목들을 설정:

#### **근접 공격 설정:**
- `Sword Prefab`: 칼 프리팹 할당
- `Sword Spawn Point`: 칼 생성 위치 Transform 할당
- `Sword Swing Duration`: 칼 휘두르는 시간 (기본값: 0.3f)

#### **원거리 공격 설정:**
- `Bow Prefab`: 활 프리팹 할당
- `Arrow Prefab`: 화살 프리팹 할당
- `Bow Spawn Point`: 활 생성 위치 Transform 할당
- `Arrow Spawn Point`: 화살 생성 위치 Transform 할당
- `Arrow Flight Duration`: 화살 날아가는 시간 (기본값: 0.3f)

#### **테스트 설정:**
- `Enable Test Mode`: 테스트 모드 활성화 (기본값: true)
- `Test Target Position`: 테스트용 타겟 위치 (기본값: Vector3(2, 0, 2))
- `Test Ranged Attack`: 원거리 공격 테스트 모드 (기본값: false)

### **2. TokenMovement 설정**
1. `TokenMovement` 컴포넌트의 Inspector에서:
   - `Attack Animation Controller`: `AttackAnimationController` 컴포넌트 할당

## 🎮 **테스트 방법**

### **K키 테스트:**
- `Enable Test Mode`가 활성화된 상태에서 K키를 누르면 테스트 실행
- `Test Ranged Attack` 체크박스로 근접/원거리 공격 전환

### **실제 게임에서:**
- `AttackAction`에서 자동으로 근접/원거리 구분하여 적절한 애니메이션 실행
- 거리가 1이면 근접 공격, 1보다 크면 원거리 공격

## 🔄 **코드 변경사항**

### **AttackAction.cs:**
- `IsMeleeAttack()` 메서드 추가로 근접/원거리 구분
- `MeleeAttackTargetFrom()` / `RangeAttackTargetFrom()` 메서드 호출

### **TokenMovement.cs:**
- 공격 애니메이션 관련 코드 제거
- `AttackAnimationController` 참조로 위임

## ⚠️ **주의사항**

1. **프리팹 설정**: 칼, 활, 화살 프리팹이 올바르게 할당되어야 함
2. **Spawn Point 설정**: 생성 위치 Transform이 올바르게 설정되어야 함
3. **컴포넌트 연결**: `TokenMovement`에서 `AttackAnimationController` 참조 필수

## 🎯 **장점**

1. **코드 분리**: 이동 로직과 공격 애니메이션 로직 분리
2. **재사용성**: `AttackAnimationController`를 다른 오브젝트에서도 사용 가능
3. **유지보수성**: 공격 애니메이션 수정 시 한 곳에서만 변경
4. **테스트 용이성**: 독립적인 테스트 기능 제공 