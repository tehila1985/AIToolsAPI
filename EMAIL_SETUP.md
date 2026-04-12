# הגדרת שליחת מיילים

## שלב 1: הוסף ל-appsettings.json

```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUser": "your-email@gmail.com",
    "SmtpPassword": "your-app-password"
  }
}
```

## שלב 2: הפעל App Password ב-Gmail

1. עבור ל-Google Account Settings
2. Security → 2-Step Verification (הפעל אם לא מופעל)
3. App passwords → צור סיסמת אפליקציה חדשה
4. העתק את הסיסמה ל-SmtpPassword

## שלב 3: הרץ את השרת

המערכת תשלח:
- מייל למנהלת: rivka7905@gmail.com עם פרטי הפנייה
- מייל ללקוח: אישור שהפנייה התקבלה

## הערות:
- אם אתה משתמש ב-Gmail אחר, שנה את SmtpUser
- אפשר להוסיף מנהלות נוספות בשורה 35 של EmailController.cs
