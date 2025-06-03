-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Хост: 127.0.0.1:3306
-- Время создания: Июн 03 2025 г., 04:27
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
(1, 48, 'ttt'),
(2, 48, 'yrty'),
(3, 49, 'hfgh');

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
(8, 22, 'New'),
(9, 22, 'new 2'),
(10, 24, 'fgdfg'),
(11, 24, 'gggg');

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
(15, 16, 26),
(22, 20, 26),
(23, 20, 25),
(24, 22, 26),
(26, 24, 27),
(27, 24, 26);

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
(16, 'test', 'tested', 0, 27),
(20, 'T', 't', 0, 28),
(21, 'G', 'g', 1, 28),
(22, 'Te', 'te', 0, 25),
(24, 'Hhh', 'hhh', 0, 25);

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
(6, 48, 'j', 'j', 26, 25, 1, 0),
(7, 48, 'b', 'b', 27, 25, 2, 0),
(9, 48, 'vvvv', 'dgh', 26, 25, 1, 0),
(10, 48, 'vb', 'bv', 26, 25, 2, 0),
(12, 48, 'g', 'g', 26, 25, 2, 0),
(16, 48, 'fg', 'fg', 26, 25, 1, 0),
(17, 48, 'd', 'd', 26, 25, 1, 0),
(18, 48, '123', '333', 26, 25, 1, 0),
(19, 48, 'gh', 'hg', 27, 25, 1, 0),
(20, 49, 'fg', 'hg', 27, 25, 3, 0);

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
(43, 22, 't1', 't1', 9, 25, 2),
(44, 22, 't2', 't2', 8, 25, 1),
(45, 22, 't3', 't3', 9, 25, 1),
(46, 22, 't5', 't5', 9, 25, 3),
(47, 22, 't4', 't4', 9, 25, 0),
(48, 24, 'dfg', 'dfg', 10, 25, 0),
(49, 24, 'fffd', 'ffffd', 10, 25, 0);

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
(45, 43, 26),
(46, 44, 26),
(47, 45, 26),
(48, 46, 26),
(49, 47, 26),
(51, 48, 26),
(52, 48, 27),
(54, 49, 27);

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
  `lastActivity` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `users`
--

INSERT INTO `users` (`id`, `email`, `password`, `salt`, `lastname`, `firstname`, `middlename`, `description`, `photo`, `lastActivity`) VALUES
(25, 'nikolay.rusanov@yandex.ru', '69b594af01f07593bf6f42602b2beb894d69e2e4fa22d3376fb4acd2af22ab16f34eec21fa082ba4108ccdb3796e8e1b42ea98312362884dfdc1290e77049a72', '691b002b5cffec0962fff6b9b20def4596bde71b72618fd826e13d7e6ad5e438', 'Rusanov', 'Nikolay', 'Alexeevich', 'novichok', NULL, '2025-06-03 01:24:41'),
(26, 'p', '136cdc5733d2e71c557db9d378dcdaad76ae1de42c7bb2877a2ec0885cb693b8d528d6aa880afe881595e757cca1bcb134cbd753b613899a436e10b80cf78bbb', 'f619018eeac6f17fed0547d63df6973a8148efe98a275a117d456374804b5631', 'Petrov', 'Petr', 'Petrovich', NULL, NULL, '2025-06-02 14:22:40'),
(27, 'i', '6d4bc3eb0d92831e9ce459fdf5f2c2b16df78c609b7687d0f1718304b2988eb1b97248adfdc657954817496ff316d85db3591aca2aa802dce46ea2ff07a238ac', 'a19d679df254cd8208fe6f2025a16773f46e76ff8b64d37dc93486f5fb9b65a2', 'Ivanov', 'Ivan', 'Ivanovich', NULL, NULL, '2025-05-27 23:03:17'),
(28, 't', 'def493f524fba11be83fb93ad4a43de58bab1f471a16f8a5a785e3587c8e1dc4f3fa1015d29e8c28877ba6e7c3c7f6b622d0d3f81f7f7a1c95fd30694b786e47', '76d14ac2e203596477bd6a06481074ebf508ef573cc9dc94dabae8381d0adbed', 'test2', 'test2', 'test2', 'tester', NULL, NULL);

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
  ADD KEY `subtasks_ibfk_1` (`chapter_id`),
  ADD KEY `creator_id` (`creator_id`);

--
-- Индексы таблицы `tasks`
--
ALTER TABLE `tasks`
  ADD PRIMARY KEY (`id`),
  ADD KEY `chapter` (`chapter_id`),
  ADD KEY `tasks_ibfk_1` (`creator_id`),
  ADD KEY `tasks_ibfk_3` (`project_id`);

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
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT для таблицы `chapters_task`
--
ALTER TABLE `chapters_task`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT для таблицы `participants`
--
ALTER TABLE `participants`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=28;

--
-- AUTO_INCREMENT для таблицы `projects`
--
ALTER TABLE `projects`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

--
-- AUTO_INCREMENT для таблицы `subtasks`
--
ALTER TABLE `subtasks`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT для таблицы `tasks`
--
ALTER TABLE `tasks`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=50;

--
-- AUTO_INCREMENT для таблицы `task_responsible`
--
ALTER TABLE `task_responsible`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=55;

--
-- AUTO_INCREMENT для таблицы `users`
--
ALTER TABLE `users`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=29;

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
  ADD CONSTRAINT `subtasks_ibfk_1` FOREIGN KEY (`chapter_id`) REFERENCES `chapters_subtask` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  ADD CONSTRAINT `subtasks_ibfk_2` FOREIGN KEY (`responsible`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  ADD CONSTRAINT `subtasks_ibfk_3` FOREIGN KEY (`task_id`) REFERENCES `tasks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  ADD CONSTRAINT `subtasks_ibfk_4` FOREIGN KEY (`creator_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;

--
-- Ограничения внешнего ключа таблицы `tasks`
--
ALTER TABLE `tasks`
  ADD CONSTRAINT `tasks_ibfk_1` FOREIGN KEY (`creator_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  ADD CONSTRAINT `tasks_ibfk_2` FOREIGN KEY (`chapter_id`) REFERENCES `chapters_task` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
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
