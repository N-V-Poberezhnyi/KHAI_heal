# Сервіс онлайн-консультацій лікарів

## Огляд

"Сервіс онлайн-консультацій лікарів" — це WPF-додаток, розроблений для автоматизації процесу онлайн-запису та консультацій між пацієнтами та лікарями. Додаток дозволяє користувачам реєструватися як пацієнти або лікарі, керувати розкладом, переглядати та бронювати прийоми, а також адміністраторам здійснювати контроль за користувачами та записами.

## Функціональні можливості

* **Реєстрація та вхід** для лікарів та пацієнтів (гості можуть реєструватися).
* **Запис на прийом** пацієнтами до обраного лікаря з урахуванням його розкладу.
* **Перегляд та управління графіком прийомів** для лікарів.
* **Перегляд власних записів** для пацієнтів та можливість їх скасування.
* **Редагування профілю** для обох типів користувачів.
* **Пошук лікарів** за спеціальністю або прізвищем.
* **Збереження даних** у форматі JSON.

## Основні вікна

* `Registration` — реєстрація нового користувача.
* `Login` — вхід у систему.
* `HomePage` — головна сторінка після входу + вікно для пошуку лікаря + вікно для вибору лікаря для запису.
* `AppointmentBookingWindow` — вікно запису на прийом (для пацієнта).
* `AccountWindow` — перегляд власних записів + профіль користувача.
* `StatusSelectionDialog` — вікно для зміни статусу запису (для лікарів).
* `UpdateProfileDoctorWindow` — вікно редагування профілю (для лікарів) .
* `UpdateProfilePatientWindow` — вікно редагування профілю (для пацієнта).

## Технології

* .NET
* C#
* WPF (Windows Presentation Foundation)
* JSON (використано `Newtonsoft.Json` для зберігання даних)

## Як користуватися

1.  Запустіть додаток.
2.  **Гість**: Зареєструйте нового користувача (лікаря або пацієнта).
3.  **Користувач**: Увійдіть у систему за своїми обліковими даними.
    * **Пацієнт**: Переглядайте список лікарів, обирайте доступний час та записуйтесь на прийом. Переглядайте та скасовуйте свої записи в "Мої записи". Редагуйте профіль.
    * **Лікар**: Переглядайте свій графік прийомів, змінюйте статуси записів. Редагуйте профіль, додавайте анкету.

## Демонстрація

* **Посилання на GitHub-репозиторій:** [https://github.com/N-V-Poberezhnyi/KHAI_heal](https://github.com/N-V-Poberezhnyi/KHAI_heal)
* **Посилання на відеоролик тестування програми:** [https://drive.google.com/file/d/1WLdpO4wL5PrLyAifsTEx7bWKf2-HJtnm/view?usp=drive_link](https://drive.google.com/file/d/1WLdpO4wL5PrLyAifsTEx7bWKf2-HJtnm/view?usp=drive_link)

## Автори

* Побережний Н.В., студент 2 курсу, група 623п
