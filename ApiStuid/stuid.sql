-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Хост: 127.0.0.1:3306
-- Время создания: Июн 10 2025 г., 21:17
-- Версия сервера: 8.0.30
-- Версия PHP: 7.2.34

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `stuid`
--

DELIMITER $$
--
-- Процедуры
--
CREATE DEFINER=`root`@`%` PROCEDURE `VerifyUserPassword` (IN `p_email` VARCHAR(50), IN `p_password` VARCHAR(128))   BEGIN
    DECLARE user_salt CHAR(64);
    DECLARE stored_password CHAR(128);
    DECLARE user_id INT;

    SELECT salt, password, id INTO user_salt, stored_password, user_id
    FROM users
    WHERE email = p_email
    LIMIT 1;

    IF user_salt IS NULL THEN
        SELECT FALSE AS IsValid, NULL AS UserId;
    ELSE
        IF SHA2(CONCAT(p_password, user_salt), 512) = stored_password THEN
            SELECT TRUE AS IsValid, user_id AS UserId;
        ELSE
            SELECT FALSE AS IsValid, NULL AS UserId;
        END IF;
    END IF;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Структура таблицы `chapters_subtask`
--

CREATE TABLE `chapters_subtask` (
  `id` int NOT NULL,
  `task_id` int NOT NULL,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `chapters_subtask`
--

INSERT INTO `chapters_subtask` (`id`, `task_id`, `name`) VALUES
(19, 83, 'Формы'),
(20, 83, 'Таблицы'),
(21, 84, 'Диаграммы'),
(22, 85, 'Тесты регистрации'),
(23, 86, 'Цвета'),
(24, 87, 'Авторизация'),
(26, 88, 'Яп');

-- --------------------------------------------------------

--
-- Структура таблицы `chapters_task`
--

CREATE TABLE `chapters_task` (
  `id` int NOT NULL,
  `project_id` int NOT NULL,
  `name` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `chapters_task`
--

INSERT INTO `chapters_task` (`id`, `project_id`, `name`) VALUES
(26, 34, 'Планирование'),
(27, 34, 'Анализ'),
(28, 34, 'Разработка'),
(29, 35, 'Подготовка тестов'),
(30, 35, 'Баг-репорты'),
(31, 36, 'Макеты'),
(32, 36, 'Цветовая палитра'),
(33, 37, 'API'),
(34, 37, 'Базы данных'),
(35, 38, 'Верстка'),
(36, 38, 'JS логика');

-- --------------------------------------------------------

--
-- Структура таблицы `participants`
--

CREATE TABLE `participants` (
  `id` int NOT NULL,
  `project_id` int NOT NULL,
  `user_id` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `participants`
--

INSERT INTO `participants` (`id`, `project_id`, `user_id`) VALUES
(36, 34, 34),
(37, 34, 35),
(38, 34, 36),
(39, 35, 37),
(40, 35, 38),
(41, 36, 34),
(42, 36, 35),
(43, 37, 36),
(44, 37, 37),
(45, 38, 38),
(46, 38, 34);

-- --------------------------------------------------------

--
-- Структура таблицы `projects`
--

CREATE TABLE `projects` (
  `id` int NOT NULL,
  `name` varchar(100) NOT NULL,
  `description` text NOT NULL,
  `is_public` tinyint(1) NOT NULL,
  `creator` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `projects`
--

INSERT INTO `projects` (`id`, `name`, `description`, `is_public`, `creator`) VALUES
(34, 'Проект Альфа', 'Разработка MVP продукта', 1, 34),
(35, 'Проект Бета', 'Тестирование и багфиксы', 0, 35),
(36, 'Проект Гамма', 'UX/UI дизайн', 1, 36),
(37, 'Проект Дельта', 'Backend разработка', 0, 37),
(38, 'Проект Эпсилон', 'Фронтенд интеграция', 1, 38);

-- --------------------------------------------------------

--
-- Структура таблицы `subtasks`
--

CREATE TABLE `subtasks` (
  `id` int NOT NULL,
  `task_id` int NOT NULL,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `responsible` int NOT NULL,
  `creator_id` int NOT NULL,
  `chapter_id` int NOT NULL,
  `position` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `subtasks`
--

INSERT INTO `subtasks` (`id`, `task_id`, `name`, `description`, `responsible`, `creator_id`, `chapter_id`, `position`) VALUES
(35, 83, 'Создать форму входа', 'HTML + CSS', 34, 34, 19, 1),
(36, 83, 'Создать форму регистрации', 'Валидация полей', 35, 34, 19, 2),
(37, 84, 'Добавить диаграмму классов', 'UML диаграмма', 36, 35, 21, 1),
(38, 85, 'Написать тест регистрации', 'Selenium', 37, 36, 22, 1),
(39, 86, 'Выбрать цветовую палитру', 'Цвета интерфейса', 38, 37, 23, 1);

-- --------------------------------------------------------

--
-- Структура таблицы `tasks`
--

CREATE TABLE `tasks` (
  `id` int NOT NULL,
  `project_id` int NOT NULL,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `chapter_id` int NOT NULL,
  `creator_id` int NOT NULL,
  `position` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `tasks`
--

INSERT INTO `tasks` (`id`, `project_id`, `name`, `description`, `chapter_id`, `creator_id`, `position`) VALUES
(83, 34, 'Собрать требования', 'Описать все функциональные модули', 26, 34, 1),
(84, 34, 'Создать техническое задание', 'Документ с описанием требований', 27, 35, 2),
(85, 35, 'Написать тест-кейсы', 'Покрытие основных сценариев', 29, 36, 1),
(86, 36, 'Создать UI-компоненты', 'Дизайн кнопок, форм и карточек', 32, 37, 1),
(87, 37, 'Реализовать REST API', 'Создать эндпоинты для клиентской части', 35, 38, 1),
(88, 34, 'Проверка', '', 26, 34, 0);

-- --------------------------------------------------------

--
-- Структура таблицы `task_responsible`
--

CREATE TABLE `task_responsible` (
  `id` int NOT NULL,
  `task_id` int NOT NULL,
  `user_id` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `task_responsible`
--

INSERT INTO `task_responsible` (`id`, `task_id`, `user_id`) VALUES
(1, 83, 34),
(2, 83, 35),
(3, 84, 36),
(4, 85, 37),
(5, 86, 38),
(6, 88, 39),
(7, 88, 34);

-- --------------------------------------------------------

--
-- Структура таблицы `users`
--

CREATE TABLE `users` (
  `id` int NOT NULL,
  `email` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `password` char(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `salt` char(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `lastname` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `firstname` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `middlename` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `photo` blob,
  `last_activity` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `users`
--

INSERT INTO `users` (`id`, `email`, `password`, `salt`, `lastname`, `firstname`, `middlename`, `description`, `photo`, `last_activity`) VALUES
(34, 'nikolay.rusanov@yandex.ru', '07640cd2018404a678e0a18f7c20adff4f9081a2651d0528d05bf642f02cd4e4c41ff49fb314bffc74621cb87b4a5e6daafd5a9d4e1213b4889f0513d1932a87', 'c8fffa8d19c6c1d96dc26f7c97ea9e40d14bd933c79f118eeaed320151849e10', 'Русанов', 'Николай', 'Алексеевич', NULL, NULL, '2025-06-10 15:39:44'),
(35, 'anna.ivanova@example.com', 'e4baaf178d35149ab664579f62c5f7d881937cefbe51d038a620601fa1da796cda6d1e7ff0c715c1e6c3bc5ddfb99f5e2648ec989e89a78ec9326d7297a54af1', '1ac8a451031223d9b8207f9faf9fe25cb6b044d7a9f87951589261054bfa218b', 'Иванова', 'Анна', 'Петровна', NULL, NULL, NULL),
(36, 'peter.smirnov@example.com', '31b82cd715ad514c5f2976f20103125aa8a026c32e023bf94945b24d040b7e9564850c145a3c87f7641f35dec9217a0223636ec859e2f3419f87bbe14b9bd000', 'a6107e9f8e34a4c8844c91385fed204b7d1610c71186e4f3772f361de690cfdb', 'Смирнов', 'Пётр', 'Сергеевич', NULL, NULL, NULL),
(37, 'maria.popova@example.com', 'e2bbc2bdf2c277961705d028f3c9ff1f5f1fabd543aa02ed9b2d2672bd74bc78216c398f1d236b3484ff9429702638019f6f26279173bddf4ab9669d6a38a8d8', '303bc57b0f51b639dbeb3c8035b4a4222b1c94474746135c8322a22af96fb48f', 'Попова', 'Мария', 'Алексеевна', NULL, NULL, NULL),
(38, 'alexander.dmitriev@example.com', 'aae25fc2689f14f6483d9e1aca447335db7aa4c0025301a7ec6847d7d885fa9631f3b2cc3a2f228ab0c1edd5703c82ff30d14da24b9b1ffce4ec8ee6140d3266', '360b4d033f4526d2d228afa44f5f97ac1c8da39420c85cb0b70cdc6a5c662b46', 'Дмитриев', 'Александр', 'Викторович', NULL, NULL, NULL),
(39, 'dmitry.kozlov@example.com', 'be409a3c6ad44cb6d3d8ed08128cf9a5764ce809a6512b5e03031218618a024c6fdb5b814ef326f1c1b55c9b8480e02f48f681703197fb93be820bc384e4d347', '4773f137be6b1fa706f8381179aa1df431dbe04bb03ab36e81acbbb0b76aa93f', 'Козлов', 'Дмитрий', 'Николаевич', NULL, NULL, NULL);

--
-- Триггеры `users`
--
DELIMITER $$
CREATE TRIGGER `hash_password_before_insert` BEFORE INSERT ON `users` FOR EACH ROW BEGIN
    -- Проверяем, что пароль не пустой и не равен NULL
    IF NEW.password IS NOT NULL AND NEW.password != '' THEN
        -- Если соль не предоставлена, генерируем новую
        IF NEW.salt IS NULL OR NEW.salt = '' OR NEW.salt = '0' THEN
            SET NEW.salt = SHA2(UUID(), 256);
        END IF;
        
        -- Хэшируем пароль с использованием соли
        SET NEW.password = SHA2(CONCAT(NEW.password, NEW.salt), 512);
    END IF;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `hash_password_before_update` BEFORE UPDATE ON `users` FOR EACH ROW BEGIN
    -- Проверяем, был ли изменён пароль
    IF NEW.password != OLD.password OR OLD.password IS NULL THEN
    
        -- Если пароль не пустой и не NULL
        IF NEW.password IS NOT NULL AND NEW.password != '' THEN
        
            -- Если соль не задана или равна '0', генерируем новую
            IF NEW.salt IS NULL OR NEW.salt = '' OR NEW.salt = '0' THEN
                SET NEW.salt = SHA2(UUID(), 256);
            END IF;
            
            -- Хэшируем новый пароль с использованием текущей соли
            SET NEW.password = SHA2(CONCAT(NEW.password, NEW.salt), 512);
        
        END IF;
    
    END IF;
END
$$
DELIMITER ;

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `chapters_subtask`
--
ALTER TABLE `chapters_subtask`
  ADD PRIMARY KEY (`id`),
  ADD KEY `chapters_subtask_ibfk_1` (`task_id`);

--
-- Индексы таблицы `chapters_task`
--
ALTER TABLE `chapters_task`
  ADD PRIMARY KEY (`id`),
  ADD KEY `project_id` (`project_id`);

--
-- Индексы таблицы `participants`
--
ALTER TABLE `participants`
  ADD PRIMARY KEY (`id`),
  ADD KEY `project_id` (`project_id`),
  ADD KEY `user_id` (`user_id`);

--
-- Индексы таблицы `projects`
--
ALTER TABLE `projects`
  ADD PRIMARY KEY (`id`),
  ADD KEY `creator` (`creator`);

--
-- Индексы таблицы `subtasks`
--
ALTER TABLE `subtasks`
  ADD PRIMARY KEY (`id`),
  ADD KEY `responsible` (`responsible`),
  ADD KEY `task_id` (`task_id`),
  ADD KEY `creator_id` (`creator_id`),
  ADD KEY `subtasks_ibfk_1` (`chapter_id`);

--
-- Индексы таблицы `tasks`
--
ALTER TABLE `tasks`
  ADD PRIMARY KEY (`id`),
  ADD KEY `tasks_ibfk_1` (`creator_id`),
  ADD KEY `tasks_ibfk_3` (`project_id`),
  ADD KEY `tasks_ibfk_2` (`chapter_id`);

--
-- Индексы таблицы `task_responsible`
--
ALTER TABLE `task_responsible`
  ADD PRIMARY KEY (`id`),
  ADD KEY `user` (`user_id`),
  ADD KEY `task_responsible_ibfk_1` (`task_id`);

--
-- Индексы таблицы `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `chapters_subtask`
--
ALTER TABLE `chapters_subtask`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=27;

--
-- AUTO_INCREMENT для таблицы `chapters_task`
--
ALTER TABLE `chapters_task`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=37;

--
-- AUTO_INCREMENT для таблицы `participants`
--
ALTER TABLE `participants`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=47;

--
-- AUTO_INCREMENT для таблицы `projects`
--
ALTER TABLE `projects`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=39;

--
-- AUTO_INCREMENT для таблицы `subtasks`
--
ALTER TABLE `subtasks`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=42;

--
-- AUTO_INCREMENT для таблицы `tasks`
--
ALTER TABLE `tasks`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=89;

--
-- AUTO_INCREMENT для таблицы `task_responsible`
--
ALTER TABLE `task_responsible`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT для таблицы `users`
--
ALTER TABLE `users`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=40;

--
-- Ограничения внешнего ключа сохраненных таблиц
--

--
-- Ограничения внешнего ключа таблицы `chapters_subtask`
--
ALTER TABLE `chapters_subtask`
  ADD CONSTRAINT `chapters_subtask_ibfk_1` FOREIGN KEY (`task_id`) REFERENCES `tasks` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `chapters_task`
--
ALTER TABLE `chapters_task`
  ADD CONSTRAINT `chapters_task_ibfk_1` FOREIGN KEY (`project_id`) REFERENCES `projects` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `participants`
--
ALTER TABLE `participants`
  ADD CONSTRAINT `participants_ibfk_1` FOREIGN KEY (`project_id`) REFERENCES `projects` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  ADD CONSTRAINT `participants_ibfk_2` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;

--
-- Ограничения внешнего ключа таблицы `projects`
--
ALTER TABLE `projects`
  ADD CONSTRAINT `projects_ibfk_1` FOREIGN KEY (`creator`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;

--
-- Ограничения внешнего ключа таблицы `subtasks`
--
ALTER TABLE `subtasks`
  ADD CONSTRAINT `subtasks_ibfk_1` FOREIGN KEY (`chapter_id`) REFERENCES `chapters_subtask` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `subtasks_ibfk_2` FOREIGN KEY (`responsible`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  ADD CONSTRAINT `subtasks_ibfk_3` FOREIGN KEY (`task_id`) REFERENCES `tasks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  ADD CONSTRAINT `subtasks_ibfk_4` FOREIGN KEY (`creator_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;

--
-- Ограничения внешнего ключа таблицы `tasks`
--
ALTER TABLE `tasks`
  ADD CONSTRAINT `tasks_ibfk_1` FOREIGN KEY (`creator_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  ADD CONSTRAINT `tasks_ibfk_2` FOREIGN KEY (`chapter_id`) REFERENCES `chapters_task` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `tasks_ibfk_3` FOREIGN KEY (`project_id`) REFERENCES `projects` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `task_responsible`
--
ALTER TABLE `task_responsible`
  ADD CONSTRAINT `task_responsible_ibfk_1` FOREIGN KEY (`task_id`) REFERENCES `tasks` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `task_responsible_ibfk_2` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
