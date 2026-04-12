# ✅ רשימת בדיקות - העלאת תמונות למנהל

## לפני הפעלת השרת

- [x] התיקיות `wwwroot/uploads/products/`, `categories/`, `styles/` נוצרו
- [x] `app.UseStaticFiles();` קיים ב-Program.cs
- [x] הוספת endpoint `[HttpPost("upload")]` ל-ProductController.cs
- [x] הוספת endpoint `[HttpPost("upload")]` ל-CategoryController.cs
- [x] הוספת endpoint `[HttpPost("upload")]` ל-StyleController.cs
- [x] כל הקוד מסומן בהערות "נוסף עבור העלאת תמונות למנהל"

## בדיקות פונקציונליות

- [ ] העלאת תמונות למוצר עובדת
- [ ] העלאת תמונה לקטגוריה עובדת
- [ ] העלאת תמונה לסגנון עובדת
- [ ] אימות מנהל עובד (משתמש רגיל לא יכול להעלות)
- [ ] התמונות נשמרות בתיקיות הנכונות
- [ ] התמונות מוצגות באפליקציה

## איך למצוא את הקוד שנוסף?

חפשי את המילים: **"נוסף עבור העלאת תמונות למנהל"**

בקבצים:
- `project1/Program.cs`
- `project1/Controllers/ProductController.cs`
- `project1/Controllers/CategoryController.cs`
- `project1/Controllers/StyleController.cs`

## איך למחוק הכל?

1. מחקי תיקייה: `wwwroot/uploads/`
2. מחקי קוד בין ההערות:
   ```
   // ===== נוסף עבור העלאת תמונות למנהל - התחלה =====
   ...
   // ===== נוסף עבור העלאת תמונות למנהל - סוף =====
   ```
3. מחקי הערה מ-Program.cs (אבל השאירי את `app.UseStaticFiles();`)

---

**קובץ מפורט:** ראי `ADMIN_IMAGE_UPLOAD_GUIDE.md`
