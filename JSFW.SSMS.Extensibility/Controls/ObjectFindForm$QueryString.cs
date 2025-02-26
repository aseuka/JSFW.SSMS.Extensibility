using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSFW
{
    partial class ObjectFindForm 
    {
        string GET_Query_R(string tableName, params string[] conditionColumns)
        {
            string qry = @"
set nocount on
/*
       - 조회 프로시져를 생성하는 쿼리
       : Table 명을 입력하면 자동으로 FK 가 되어 있는 테이블까지 Join문으로 작성한다.
       : @conditionColumns 에 조회 프로시져의 파라미터대상 컬럼명을 등록하면 자동생성한다. 
*/
declare @작성자 varchar(100) = ''
declare @tableName nvarchar(777) = '$JS$TABLE_NAME$FW$'
declare @tableID int = object_id( @tableName )
declare @schemaName varchar(10) = isnull( schema_name() , 'dbo') 
-- sp_help 또는 columnInfo sp를 호출해서 대상 컬럼명을 그대로 복사해서 등록하면 됨.
declare @conditionColumns varchar(max) = '
$JS$WHERE_CONDITION$FW$
'   
declare @MaxColumnNameLength int = 30
declare @MaxTypeNameLength int = 23

-- 조회용 마지막 컬럼 : 
declare @select_last_columnName varchar(100) = ''
-- 파라미터 마지막 변수명 + 타입명:
declare @parameter_lastInfo varchar(100) = '' 
 

select @MaxColumnNameLength = max(len( name )) 
from sys.columns 

select @MaxTypeNameLength = max( len( name )) + len('( 18,0 )')
from sys.types
 
--select @tableName, @tableID

declare @selectSPheader varchar(max) = '' --  @conditionColumns 컬럼명에 등록된 대상으로 parameter 생성함.
declare @selectSPbody   varchar(max) = ''
declare @selectSPfrom   varchar(max) = ''
declare @selectSPwhere  varchar(max) = ''
declare @selectSPfooter varchar(max) = ''

declare @SPNAME varchar( 255 ) = ''

if( len( @tableName ) > 3 and len( @tableName ) > 5)
    set @SPNAME = QUOTENAME( @schemaName ) + '.' + QUOTENAME(  left(  @tableName, 3) + 'PR_' + right( @tableName, len(@tableName) - 5) +'_SELECT' )
else
    set @SPNAME = QUOTENAME( @schemaName ) + '.' + QUOTENAME( @tableName + 'PR_SELECT') 

/* SELECT SP 관련 정보 셋팅           *******************************************************************************************************/
set @selectSPheader = '
/******************************************************************************
 기  능 : 
 작성자 : '+ @작성자 +'
 작성일 : '+ Convert( varchar, GetDate(), 102 ) +  '
 Parameters: 
 Return Value : 
*******************************************************************************/
CREATE
PROCEDURE ' + @SPNAME + '
('

set @selectSPbody = '
    SELECT  '
         
set @selectSPfrom = '
      FROM  '

set @selectSPwhere = '
     WHERE  '

/************************************************************************************************************* SELECT SP 관련 정보 셋팅     */
 
declare @joinTableInfo table ( tableName varchar(777), alias varchar(2))

insert into @joinTableInfo
select @tableName , 'a' 
union all
select object_name( f.referenced_object_id ), char( ascii('a') + Row_Number() over ( order by object_name( f.referenced_object_id )))
from   sys.columns c
left 
outer  join  sys.foreign_key_columns f    
on     c.object_id = f.parent_object_id and c.column_id = f.parent_column_id 
where  c.object_id = @tableID and f.constraint_column_id is not null
group by object_name( f.referenced_object_id )

--declare @hasJoinTableInfo int = 0
--select @hasJoinTableInfo = isnull( count(tablename), 0)
--from @joinTableInfo

;with tableInfo as
( 
    select
            c.object_id, c.name, c.column_id, c.system_type_id, c.max_length, c.precision, c.scale, c.is_nullable, c.is_identity,
            case when isnull( index_col( object_name( c.object_id) , i.index_id, i.index_column_id ),'') <> '' then 'PK' else '' end as IsPK,
            case when isnull( f.constraint_column_id , '' ) <> '' then 'FK' else '' end as IsFK,
            e.value
    from    sys.columns c
    left 
    outer   join sys.index_columns i    
    on      c.object_id = i.object_id and c.column_id = i.column_id
    left outer join  sys.foreign_key_columns f    
    on c.object_id = f.parent_object_id and c.column_id = f.parent_column_id  
    left outer join sys.extended_properties e        
    on c.Object_id = e.MAJOR_ID and c.Column_Id = e.Minor_id and e.CLASS = 1        
    where   c.object_id = @tableID
)  
  
-- 테이블 정보!!
select 
/* SELECT SP 관련 정보 셋팅           *******************************************************************************************************/
     @selectSPheader = @selectSPheader + 
     CASE WHEN CHARINDEX( c.name , @conditionColumns ) >= 1 THEN '
    ' +'@'+CONVERT( VARCHAR,  c.name ) +' ' +
        replicate(' ', @MaxColumnNameLength - len( '@'+CONVERT( VARCHAR,  c.name ) +' ' )) + 
        --데이타 타입
        + UPPER(type_name(c.system_type_id)) +  
        CASE WHEN UPPER(type_name(c.system_type_id)) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR') THEN CASE c.max_length WHEN -1 THEN '(MAX)' ELSE '('+ CONVERT( VARCHAR, c.max_length / ( CASE WHEN LEFT(type_name(c.system_type_id),1) = 'N' THEN 2 ELSE 1 END ) ) +')' END         
             WHEN UPPER(type_name(c.system_type_id)) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.scale)+')'        
             ELSE ''         
        END + ',' +
       -- 별칭
        replicate(' ', @MaxTypeNameLength - len( ' ' + UPPER(type_name(c.system_type_id)) +  
        CASE WHEN UPPER(type_name(c.system_type_id)) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR') THEN CASE c.max_length WHEN -1 THEN '(MAX)' ELSE '('+ CONVERT( VARCHAR, c.max_length / ( CASE WHEN LEFT(type_name(c.system_type_id),1) = 'N' THEN 2 ELSE 1 END ) ) +')' END         
                WHEN UPPER(type_name(c.system_type_id)) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.scale)+')'        
                ELSE ''         
        END + ',' )) + 
        isnull( '-- ' + CONVERT( VARCHAR,  c.value  ), '') ELSE '' END,
     -- 파라미터 콤마값 제거 후 주석정렬에 사용될 값.
     @parameter_lastInfo = 
     CASE WHEN CHARINDEX( c.name , @conditionColumns ) >= 1 THEN 
        replicate('-', @MaxTypeNameLength - len( ' ' + UPPER(type_name(c.system_type_id)) +  
        CASE WHEN UPPER(type_name(c.system_type_id)) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR') THEN CASE c.max_length WHEN -1 THEN '(MAX)' ELSE '('+ CONVERT( VARCHAR, c.max_length / ( CASE WHEN LEFT(type_name(c.system_type_id),1) = 'N' THEN 2 ELSE 1 END ) ) +')' END         
                WHEN UPPER(type_name(c.system_type_id)) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.scale)+')'        
                ELSE ''         
        END + ',' ))
     else @parameter_lastInfo end, 

     @selectSPbody = @selectSPbody + isnull((select alias + '.' from @joinTableInfo where tableName = object_name( c.object_id)), '') +  c.name + ',' +
     REPLICATE( ' ', @MaxColumnNameLength + @MaxTypeNameLength - len( isnull((select alias + '.' from @joinTableInfo where tableName = object_name( c.object_id)), '') +  c.name + ',')) + isnull( '-- '+ convert( varchar, c.value ), '') + '
            ',
     @select_last_columnName = name,

     @selectSPwhere = @selectSPwhere + case when CHARINDEX( c.name , @conditionColumns ) >= 1 then c.name + ' = @' + c.name + '
       AND  ' else '' end
/************************************************************************************************************* SELECT SP 관련 정보 셋팅     */ 
from tableInfo c 

select 
       @selectSPbody = @selectSPbody + 
       isnull((select alias + '.' from @joinTableInfo where tableName = object_name( cc.object_id)), '') +  cc.name + ',' + 
       replicate(' ', @MaxColumnNameLength + @MaxTypeNameLength - len(isnull((select alias + '.' from @joinTableInfo where tableName = object_name( cc.object_id)), '') +  cc.name + ',') ) + isnull( '-- '+ convert( varchar, cc.value ), '') + '
            ', 
       @select_last_columnName = name
from (   
    select
            c.object_id, c.name, c.column_id, c.system_type_id, c.max_length, c.precision, c.scale, c.is_nullable, c.is_identity,
            case when isnull( index_col( object_name( c.object_id) , i.index_id, i.index_column_id ),'') <> '' then 'PK' else '' end as IsPK,
            case when isnull( f.constraint_column_id , '' ) <> '' then 'FK' else '' end as IsFK,
            e.value
    from    sys.columns c
    left 
    outer   join sys.index_columns i    
    on      c.object_id = i.object_id and c.column_id = i.column_id
    left outer join  sys.foreign_key_columns f    
    on c.object_id = f.parent_object_id and c.column_id = f.parent_column_id  
    left outer join sys.extended_properties e        
    on c.Object_id = e.MAJOR_ID and c.Column_Id = e.Minor_id and e.CLASS = 1        
    where   c.object_id = ( select object_id( tablename ) from @joinTableInfo where alias <> 'a' )
) cc

------ FROM - join
;with joinInfo as
( 
    select c.object_id, object_name( c.object_id ) as tableName, c.name, c.column_id, f.referenced_object_id, object_name( f.referenced_object_id ) as refTableName, f.referenced_column_id, ( select name from sys.columns where object_id = f.referenced_object_id and column_id = f.referenced_column_id) as refColumnName
    from   sys.columns c
    left 
    outer  join  sys.foreign_key_columns f    
    on     c.object_id = f.parent_object_id and c.column_id = f.parent_column_id 
    where  c.object_id = @tableID and f.constraint_column_id is not null
), Info as
(
    select  '1' AS ODR, r.alias as rAlias, r.tableName as rTableName, '' AS RCOLNAME, c.alias, c.tableName, '' AS COLNAME
    from    joinInfo j left outer join @joinTableInfo r
    on      j.refTableName = r.tableName
    left outer join @joinTableInfo c
    on      j.tableName = c.tableName 
    union   
    select  '2', r.alias, r.tableName, j.refColumnName, c.alias, c.tableName, j.name
    from    joinInfo j left outer join @joinTableInfo r
    on      j.refTableName = r.tableName
    left outer join @joinTableInfo c
    on      j.tableName = c.tableName 
)

select  @selectSPfrom =  @selectSPfrom + 
        case when odr = '1' then 
            QUOTENAME( @schemaName )+ '.' + QUOTENAME( tableName) + ' as ' + alias +
    '
      LEFT  OUTER JOIN '+ QUOTENAME( @schemaName )+ '.' + QUOTENAME( rTableName) + ' as ' + rAlias + '
        ON  '
        else  rAlias + '.'+ RCOLNAME + ' = ' + alias + '.' + COLNAME +
        case when LEAD(COLNAME, 1,0) OVER (ORDER BY ODR) <> '0' then 
    '
       AND  ' else '' end
        end
 
from  Info /* 2012 보다 하위 버젼은 LEAD함수 대신 self조인으로 대체가능.*/
--(
--    select  '1' AS ODR, r.alias as rAlias, r.tableName as rTableName, '' AS RCOLNAME, c.alias, c.tableName, '' AS COLNAME
--    from    joinInfo j left outer join @joinTableInfo r
--    on      j.refTableName = r.tableName
--    left outer join @joinTableInfo c
--    on      j.tableName = c.tableName 
--    union   
--    select  '2', r.alias, r.tableName, j.refColumnName, c.alias, c.tableName, j.name
--    from    joinInfo j left outer join @joinTableInfo r
--    on      j.refTableName = r.tableName
--    left outer join @joinTableInfo c
--    on      j.tableName = c.tableName 
--) as info 
 
if @@rowcount = 0
begin
    set @selectSPfrom = @selectSPfrom + QUOTENAME( @schemaName )+ '.' + QUOTENAME( @tableName) + ' as a'
end

-- 콤마제거용
declare @lastCommaIndex int = 0
declare @Index int = 0
   
while( @Index < len( @selectSPheader ))
begin 
    if(  charindex( ',', @selectSPheader, @Index) > @lastCommaIndex )
    begin
        set @lastCommaIndex = charindex( ',', @selectSPheader, @Index)
        set @Index = @lastCommaIndex + 1
    end
    else 
        break
end


if( 0 < @lastCommaIndex and @lastCommaIndex < len( @selectSPheader ))
set @selectSPheader = left( @selectSPheader, @lastCommaIndex-1) + replicate(' ', len(@parameter_lastInfo) + len(',')) + Ltrim( right( @selectSPheader, len(@selectSPheader) - @lastCommaIndex) )
 
-- 콤마제거
set @selectSPheader = rtrim( @selectSPheader )
set @selectSPheader = left( @selectSPheader, len( @selectSPheader ) - len(',') ) 
  

/* SELECT SP 관련 정보 셋팅           *******************************************************************************************************/
set @selectSPheader =  @selectSPheader + '
)
AS

SET NOCOUNT ON
SET XACT_ABORT ON
SET TRANSACTION ISOLATION LEVEL READ COMMITTED

BEGIN
'
set @selectSPfooter = '
END
'

set @lastCommaIndex = 0
set  @Index = 0
 
while( @Index < len( @selectSPbody ))
begin 
    if(  charindex( ',', @selectSPbody, @Index) > @lastCommaIndex )
    begin
        set @lastCommaIndex = charindex( ',', @selectSPbody, @Index)
        set @Index = @lastCommaIndex + 1
    end
    else 
        break
end

set @selectSPbody = left( @selectSPbody, @lastCommaIndex-1) + replicate(' ', @MaxColumnNameLength + @MaxTypeNameLength - len(@select_last_columnName) - len(' ,')) +  Ltrim( right( @selectSPBody, len(@selectSPBody) - @lastCommaIndex - 1) )

set @selectSPwhere = left( @selectSPwhere, len( @selectSPwhere ) - len( '
       AND  ')) 

/************************************************************************************************************* SELECT SP 관련 정보 셋팅     */ 

print @selectSPheader
print @selectSPbody
print @selectSPfrom
print @selectSPwhere
print @selectSPfooter
";
            return qry.Replace("$JS$TABLE_NAME$FW$", tableName).Replace("$JS$WHERE_CONDITION$FW$", string.Join(Environment.NewLine, conditionColumns));
            //return qry;
        
        }
         
        string GET_Query_CUD(string tableName)
        {
            string qry = @"

set nocount on

/*
       - 입력/ 삭제/ 수정 프로시져를 생성하는 쿼리
       : Table 명을 입력하면 자동으로 입력/ 삭제/ 수정 프로시져를 생성한다.
*/
declare @작성자 varchar(100) = ''
declare @tableName nvarchar(777) = '$JS$TABLE_NAME$FW$' 
declare @tableID int = object_id( @tableName )
declare @schemaName varchar(10) = isnull( schema_name() , 'dbo')
-- select @tableName, @tableID
declare @MaxColumnNameLength int = 30
declare @MaxTypeNameLength int = 23

select @MaxColumnNameLength = max(len( name )) 
from sys.columns 

select @MaxTypeNameLength = max( len( name )) + len('( 18,0 )')
from sys.types
  
declare @insertSPheader varchar( max ) = ''
declare @insertSPfooter varchar( max ) = ''
declare @insertSPbody varchar( max ) = ''
declare @valuesSPbody varchar( max ) = ''
 
-- 입력 마지막 컬럼 : 
declare @insert_last_columnName varchar(100) = ''
-- 입력 파라미터 마지막 변수명 + 타입명:
declare @insert_parameter_lastInfo varchar(100) = '' 
 
declare @deleteSPheader varchar( max ) = ''
declare @deleteSPfooter varchar( max ) = ''
declare @deleteSPbody varchar( max ) = ''
declare @deleteSPwhere varchar( max ) = ''

-- 삭제 마지막 컬럼 : 
declare @delete_last_columnName varchar(100) = ''
-- 삭제 파라미터 마지막 변수명 + 타입명:
declare @delete_parameter_lastInfo varchar(100) = '' 

declare @updateSPheader varchar( max ) = ''
declare @updateSPfooter varchar( max ) = ''
declare @updateSPset varchar( max ) = ''
declare @updateSPwhere varchar( max ) = ''

declare @hasPK int = 0

-- 수정 마지막 컬럼 : 
declare @update_last_columnName varchar(100) = ''
-- 수정 파라미터 마지막 변수명 + 타입명:
declare @update_parameter_lastInfo varchar(100) = '' 

 
/* INSERT SP 관련 정보 셋팅           *******************************************************************************************************/
set @insertSPheader = '
/******************************************************************************
 기  능 : 
 작성자 : '+ @작성자 +'
 작성일 : '+ Convert( varchar, GetDate(), 102 ) +  '
 Parameters: 
 Return Value : 
*******************************************************************************/
CREATE
PROCEDURE ' + QUOTENAME( @schemaName ) + '.' + QUOTENAME(  left(  @tableName, 3) + 'PR_' + right( @tableName, len(@tableName) - 5) + '_INSERT' ) + '
('  
 
set @insertSPbody = '
    INSERT  INTO ' + QUOTENAME( @schemaName ) + '.' + QUOTENAME(@tableName) + '
            ('
         
set @valuesSPbody = '
    VALUES
            ('
/************************************************************************************************************* INSERT SP 관련 정보 셋팅     */
/* DELETE SP 관련 정보 셋팅           *******************************************************************************************************/
set @deleteSPheader = '
/******************************************************************************
 기  능 : 
 작성자 : '+ @작성자 +'
 작성일 : '+ Convert( varchar, GetDate(), 102 ) +  '
 Parameters: 
 Return Value : 
*******************************************************************************/
CREATE
PROCEDURE ' + QUOTENAME( @schemaName ) + '.' + QUOTENAME(  left(  @tableName, 3) + 'PR_' + right( @tableName, len(@tableName) - 5) + '_DELETE' ) + '
('  
 
set @deleteSPbody = '
    DELETE  FROM ' + QUOTENAME( @schemaName ) + '.' + QUOTENAME(@tableName) 

/************************************************************************************************************* DELETE SP 관련 정보 셋팅     */
/* UPDATE SP 관련 정보 셋팅           *******************************************************************************************************/
set @updateSPheader = '
/******************************************************************************
 기  능 : 
 작성자 : '+ @작성자 +'
 작성일 : '+ Convert( varchar, GetDate(), 102 ) +  '
 Parameters: 
 Return Value : 
*******************************************************************************/
CREATE
PROCEDURE ' + QUOTENAME( @schemaName ) + '.' + QUOTENAME(  left(  @tableName, 3) + 'PR_' + right( @tableName, len(@tableName) - 5) + '_UPDATE' ) + '
('  
 
set @updateSPset = '
    UPDATE  ' + QUOTENAME( @schemaName ) + '.' + QUOTENAME(@tableName) + '
       SET  '
         
set @updateSPwhere = ''
/************************************************************************************************************* UPDATE SP 관련 정보 셋팅     */
  
;with tableInfo as
( 
    select
            c.object_id, c.name, c.column_id, c.system_type_id, c.max_length, c.precision, c.scale, c.is_nullable, c.is_identity,
            case when isnull( index_col( object_name( c.object_id) , i.index_id, i.index_column_id ),'') <> '' then 'PK' else '' end as IsPK,
            case when isnull( f.constraint_column_id , '' ) <> '' then 'FK' else '' end as IsFK,
            e.value
    from    sys.columns c
    left 
    outer   join sys.index_columns i    
    on      c.object_id = i.object_id and c.column_id = i.column_id
    left outer join  sys.foreign_key_columns f    
    on c.object_id = f.parent_object_id and c.column_id = f.parent_column_id  
    left outer join sys.extended_properties e        
    on c.Object_id = e.MAJOR_ID and c.Column_Id = e.Minor_id and e.CLASS = 1        
    where   c.object_id = @tableID
), 
joinInfo as
( 
    select c.object_id, object_name( c.object_id ) as tableName, c.name, c.column_id, object_name( f.referenced_object_id ) as refTableName, f.referenced_column_id, ( select name from sys.columns where object_id = f.referenced_object_id and column_id = f.referenced_column_id) as refColumnName
    from   sys.columns c
    left 
    outer  join  sys.foreign_key_columns f    
    on     c.object_id = f.parent_object_id and c.column_id = f.parent_column_id 
    where  c.object_id = @tableID and f.constraint_column_id is not null
)

-- 테이블 정보!!
select  
/* INSERT SP 관련 정보 셋팅           *******************************************************************************************************/
     @insertSPheader = @insertSPheader + 
     CASE WHEN c.is_identity = 0 THEN '
    ' +'@'+CONVERT( VARCHAR,  c.name )+' ' +
        replicate(' ', @MaxColumnNameLength - len( '@'+CONVERT( VARCHAR,  c.name ) +' ' )) +  
        UPPER(type_name(c.system_type_id)) +  
        CASE WHEN UPPER(type_name(c.system_type_id)) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR') THEN CASE c.max_length WHEN -1 THEN '(MAX)' ELSE '('+ CONVERT( VARCHAR, c.max_length / ( CASE WHEN LEFT(type_name(c.system_type_id),1) = 'N' THEN 2 ELSE 1 END ) ) +')' END         
             WHEN UPPER(type_name(c.system_type_id)) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.scale)+')'        
             ELSE ''         
        END + ',' +
        -- 별칭
        replicate(' ', @MaxTypeNameLength - len( ' ' + UPPER(type_name(c.system_type_id)) +  
        CASE WHEN UPPER(type_name(c.system_type_id)) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR') THEN CASE c.max_length WHEN -1 THEN '(MAX)' ELSE '('+ CONVERT( VARCHAR, c.max_length / ( CASE WHEN LEFT(type_name(c.system_type_id),1) = 'N' THEN 2 ELSE 1 END ) ) +')' END         
                WHEN UPPER(type_name(c.system_type_id)) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.scale)+')'        
                ELSE ''         
        END + ',' )) + 
        isnull( '-- ' + CONVERT( VARCHAR,  c.value  ), '') ELSE '' END,
         @insert_parameter_lastInfo = 
     CASE WHEN c.is_identity = 0 THEN 
        replicate('-', @MaxTypeNameLength - len( ' ' + UPPER(type_name(c.system_type_id)) +  
        CASE WHEN UPPER(type_name(c.system_type_id)) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR') THEN CASE c.max_length WHEN -1 THEN '(MAX)' ELSE '('+ CONVERT( VARCHAR, c.max_length / ( CASE WHEN LEFT(type_name(c.system_type_id),1) = 'N' THEN 2 ELSE 1 END ) ) +')' END         
                WHEN UPPER(type_name(c.system_type_id)) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.scale)+')'        
                ELSE ''         
        END + ',' ))
     else @insert_parameter_lastInfo end, 

        @insertSPbody = @insertSPbody +  CASE WHEN c.is_identity = 0 THEN '
                ' + c.name + ',' + REPLICATE(' ', @MaxColumnNameLength + @MaxTypeNameLength - len( c.name )) + isnull( '-- '+ convert( varchar, c.value ), '') ELSE '' END, 
        @valuesSPbody = @valuesSPbody + CASE WHEN c.is_identity = 0 THEN '
                @' + c.name + ',' ELSE '' END,  
        @insert_last_columnName = CASE WHEN c.is_identity = 0 THEN c.name ELSE @insert_last_columnName END,

/************************************************************************************************************* INSERT SP 관련 정보 셋팅     */
/* DELETE SP 관련 정보 셋팅           *******************************************************************************************************/

    @deleteSPheader = @deleteSPheader + CASE WHEN c.IsPK = 'PK' THEN '
    ' +'@'+CONVERT( VARCHAR,  c.name )+' ' +
        replicate(' ', @MaxColumnNameLength - len( '@'+CONVERT( VARCHAR,  c.name ) +' ' )) +  
        UPPER(type_name(c.system_type_id)) + 
        CASE WHEN UPPER(type_name(c.system_type_id)) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR') THEN CASE c.max_length WHEN -1 THEN '(MAX)'         
            ELSE '('+ CONVERT( VARCHAR, c.max_length / ( CASE WHEN LEFT(type_name(c.system_type_id),1) = 'N' THEN 2 ELSE 1 END ) ) +')'         
          END         
           WHEN UPPER(type_name(c.system_type_id)) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.scale)+')'        
           ELSE ''         
      END + ',' +
        -- 별칭
        replicate(' ', @MaxTypeNameLength - len( ' ' + UPPER(type_name(c.system_type_id)) +  
        CASE WHEN UPPER(type_name(c.system_type_id)) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR') THEN CASE c.max_length WHEN -1 THEN '(MAX)' ELSE '('+ CONVERT( VARCHAR, c.max_length / ( CASE WHEN LEFT(type_name(c.system_type_id),1) = 'N' THEN 2 ELSE 1 END ) ) +')' END         
                WHEN UPPER(type_name(c.system_type_id)) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.scale)+')'        
                ELSE ''         
        END + ',' )) + 
        isnull( '-- ' + CONVERT( VARCHAR,  c.value  ), '') ELSE '' END,
    @delete_parameter_lastInfo = 
     CASE WHEN c.IsPK = 'PK' THEN 
        replicate('-', @MaxTypeNameLength - len( ' ' + UPPER(type_name(c.system_type_id)) +  
        CASE WHEN UPPER(type_name(c.system_type_id)) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR') THEN CASE c.max_length WHEN -1 THEN '(MAX)' ELSE '('+ CONVERT( VARCHAR, c.max_length / ( CASE WHEN LEFT(type_name(c.system_type_id),1) = 'N' THEN 2 ELSE 1 END ) ) +')' END         
                WHEN UPPER(type_name(c.system_type_id)) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.scale)+')'        
                ELSE ''         
        END + ',' ))
     else @delete_parameter_lastInfo end, 

        @deleteSPwhere = @deleteSPwhere + CASE WHEN c.ISPK = 'PK' THEN c.name + ' = @'+ c.name +'
       AND  ' ELSE '' END, -- 탭1개 + 공백3개 후 AND
        @delete_last_columnName = CASE WHEN c.IsPK = 'PK' THEN c.name ELSE @delete_last_columnName END,
/************************************************************************************************************* DELETE SP 관련 정보 셋팅     */
/* UPDATE SP 관련 정보 셋팅           *******************************************************************************************************/
     @updateSPheader = @updateSPheader + '
    ' +'@'+CONVERT( VARCHAR,  c.name )+' ' +
      replicate(' ', @MaxColumnNameLength - len( '@'+CONVERT( VARCHAR,  c.name ) +' ' )) +  
      UPPER(type_name(c.system_type_id)) +  
      CASE WHEN UPPER(type_name(c.system_type_id)) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR') THEN CASE c.max_length WHEN -1 THEN '(MAX)' ELSE '('+ CONVERT( VARCHAR, c.max_length / ( CASE WHEN LEFT(type_name(c.system_type_id),1) = 'N' THEN 2 ELSE 1 END ) ) +')' END         
           WHEN UPPER(type_name(c.system_type_id)) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.scale)+')'        
           ELSE ''         
      END + ',' +
        -- 별칭
      replicate(' ', @MaxTypeNameLength - len( ' ' + UPPER(type_name(c.system_type_id)) +  
      CASE WHEN UPPER(type_name(c.system_type_id)) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR') THEN CASE c.max_length WHEN -1 THEN '(MAX)' ELSE '('+ CONVERT( VARCHAR, c.max_length / ( CASE WHEN LEFT(type_name(c.system_type_id),1) = 'N' THEN 2 ELSE 1 END ) ) +')' END         
           WHEN UPPER(type_name(c.system_type_id)) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.scale)+')'        
           ELSE ''         
      END + ',' )) + 
      isnull( '-- ' + CONVERT( VARCHAR,  c.value ), ''),

      @update_parameter_lastInfo =  
        replicate('-', @MaxTypeNameLength - len( ' ' + UPPER(type_name(c.system_type_id)) +  
        CASE WHEN UPPER(type_name(c.system_type_id)) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR') THEN CASE c.max_length WHEN -1 THEN '(MAX)' ELSE '('+ CONVERT( VARCHAR, c.max_length / ( CASE WHEN LEFT(type_name(c.system_type_id),1) = 'N' THEN 2 ELSE 1 END ) ) +')' END         
                WHEN UPPER(type_name(c.system_type_id)) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.scale)+')'        
                ELSE ''         
        END + ',' )) , 

        @updateSPset = @updateSPset + c.name + ' = @'+ c.name + ',
            ',
        @updateSPwhere = @updateSPwhere + CASE WHEN c.isPK = 'PK' THEN c.name + ' = @' + c.name + '
       AND  ' ELSE '' END
/************************************************************************************************************* UPDATE SP 관련 정보 셋팅     */
       ,@hasPK = case when c.isPK = 'PK' and @hasPK = 0 then 1 
		              else @hasPK
				 end

from tableInfo c 
  
/* INSERT SP 관련 정보 셋팅           *******************************************************************************************************/


-- 콤마제거용
declare @lastCommaIndex int = 0
declare @Index int = 0
   
while( @Index < len( @insertSPheader ))
begin 
    if(  charindex( ',', @insertSPheader, @Index) > @lastCommaIndex )
    begin
        set @lastCommaIndex = charindex( ',', @insertSPheader, @Index)
        set @Index = @lastCommaIndex + 1
    end
    else 
        break
end

set @insertSPheader = left( @insertSPheader, @lastCommaIndex-1) + replicate(' ', len(@insert_parameter_lastInfo) + len(',')) + Ltrim( right( @insertSPheader, len(@insertSPheader) - @lastCommaIndex) )
set @insertSPheader = @insertSPheader + '
)
AS

SET NOCOUNT ON
SET XACT_ABORT ON
SET TRANSACTION ISOLATION LEVEL READ COMMITTED

BEGIN
'
set @insertSPfooter = '
END
'


set @lastCommaIndex = 0
set  @Index = 0
 
while( @Index < len( @insertSPbody ))
begin 
    if(  charindex( ',', @insertSPbody, @Index) > @lastCommaIndex )
    begin
        set @lastCommaIndex = charindex( ',', @insertSPbody, @Index)
        set @Index = @lastCommaIndex + 1
    end
    else 
        break
end
  
set @insertSPbody = left( @insertSPbody, @lastCommaIndex-1) + ' ' + replicate(' ', @MaxColumnNameLength + @MaxTypeNameLength- len( @insert_last_columnName )) +  Ltrim( right( @insertSPbody, len(@insertSPbody) - @lastCommaIndex) )
set @insertSPbody = @insertSPbody + '
            )'

set @valuesSPbody = left( @valuesSPbody, len( @valuesSPbody ) - len( ',')) + '
            )'

/************************************************************************************************************* INSERT SP 관련 정보 셋팅     */

/* DELETE SP 관련 정보 셋팅           *******************************************************************************************************/
set @lastCommaIndex = 0
set  @Index = 0
 
while( @Index < len( @deleteSPheader ))
begin 
    if(  charindex( ',', @deleteSPheader, @Index) > @lastCommaIndex )
    begin
        set @lastCommaIndex = charindex( ',', @deleteSPheader, @Index)
        set @Index = @lastCommaIndex + 1
    end
    else 
        break
end
  
if( 0 < @lastCommaIndex )
set @deleteSPheader = left( @deleteSPheader, @lastCommaIndex-1) + replicate(' ', len(@delete_parameter_lastInfo) + len(',')) + Ltrim( right( @deleteSPheader, len(@deleteSPheader) - @lastCommaIndex) )
set @deleteSPheader = @deleteSPheader + '
)
AS

SET NOCOUNT ON
SET XACT_ABORT ON
SET TRANSACTION ISOLATION LEVEL READ COMMITTED

BEGIN
'
set @deleteSPfooter = '
END

'

if( @hasPK <> 0 )
    set @deleteSPwhere = '
     WHERE  ' + left( @deleteSPwhere, len( @deleteSPwhere ) - len('AND'))
/************************************************************************************************************* DELETE SP 관련 정보 셋팅     */

/* UPDATE SP 관련 정보 셋팅           *******************************************************************************************************/
set @lastCommaIndex = 0
set  @Index = 0
 
while( @Index < len( @updateSPheader ))
begin 
    if(  charindex( ',', @updateSPheader, @Index) > @lastCommaIndex )
    begin
        set @lastCommaIndex = charindex( ',', @updateSPheader, @Index)
        set @Index = @lastCommaIndex + 1
    end
    else 
        break
end
  
if( 0 < @lastCommaIndex )
set @updateSPheader = left( @updateSPheader, @lastCommaIndex-1) + replicate(' ', len(@update_parameter_lastInfo) + len(',')) + Ltrim( right( @updateSPheader, len(@updateSPheader) - @lastCommaIndex) )
set @updateSPheader = @updateSPheader + '
)
AS

SET NOCOUNT ON
SET XACT_ABORT ON
SET TRANSACTION ISOLATION LEVEL READ COMMITTED

BEGIN
'
set @updateSPfooter = '
END
'
 
set @updateSPset = left( Rtrim( @updateSPset), len( Rtrim( @updateSPset ) ) - len(char(10)) - len(char(13)) - len( ','))
if( @hasPK <> 0 )
    set @updateSPwhere ='
     WHERE  ' + left( @updateSPwhere, len( @updateSPwhere ) - len( 'AND'))
/************************************************************************************************************* UPDATE SP 관련 정보 셋팅     */
  
print @insertSPheader
print @insertSPbody
print @valuesSPbody
print @insertSPfooter
 
print @deleteSPheader
print @deleteSPbody
print @deleteSPwhere
print @deleteSPfooter
 
print @updateSPheader
print @updateSPset
print @updateSPwhere
print @updateSPfooter
 
"; 
            return qry.Replace("$JS$TABLE_NAME$FW$", tableName);
            //return qry;
        }

        string GET_QUERY_SP_HELPTXT()
        {
            string qry = @"
USE [master]
GO
/****** Object:  StoredProcedure [dbo].[SP_HELPTXT]    Script Date: 2016-06-14 오전 11:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE
PROCEDURE [dbo].[SP_HELPTXT]  
	@objname nvarchar(776)  
   ,@columnname sysname = NULL  
as  
  
SET NOCOUNT ON  
  
declare @dbname sysname  
,@objid int  
,@BlankSpaceAdded   int  
,@BasePos       int  
,@CurrentPos    int  
,@TextLength    int  
,@LineId        int  
,@AddOnLen      int  
,@LFCR          int --lengths of line feed carriage return  
,@DefinedLength int  
  
/* NOTE: Length of @SyscomText is 4000 to replace the length of  
** text column in syscomments.  
** lengths on @Line, #CommentText Text column and  
** value for @DefinedLength are all 255. These need to all have  
** the same values. 255 was selected in order for the max length  
** display using down level clients  
*/  
,@SyscomText nvarchar(4000)  
,@Line          nvarchar(255)  
  
select @DefinedLength = 255  
select @BlankSpaceAdded = 0 /*Keeps track of blank spaces at end of lines. Note Len function ignores  
                             trailing blank spaces*/  
CREATE TABLE #CommentText  
(LineId int  
 ,Text  nvarchar(255) collate database_default)  
  
/*  
**  Make sure the @objname is local to the current database.  
*/  
select @dbname = parsename(@objname,3)  
if @dbname is null  
 select @dbname = db_name()  
else if @dbname <> db_name()  
        begin  
                raiserror(15250,-1,-1)  
                return (1)  
        end  
  
/*  
**  See if @objname exists.  
*/  
select @objid = object_id(@objname)  
if (@objid is null)  
        begin  
  raiserror(15009,-1,-1,@objname,@dbname)  
  return (1)  
        end  
  
-- If second parameter was given.  
if ( @columnname is not null)  
    begin  
        -- Check if it is a table  
        if (select count(*) from sys.objects where object_id = @objid and type in ('S ','U ','TF'))=0  
            begin  
                raiserror(15218,-1,-1,@objname)  
                return(1)  
            end  
        -- check if it is a correct column name  
        if ((select 'count'=count(*) from sys.columns where name = @columnname and object_id = @objid) =0)  
            begin  
                raiserror(15645,-1,-1,@columnname)  
                return(1)  
            end  
    if (ColumnProperty(@objid, @columnname, 'IsComputed') = 0)  
  begin  
   raiserror(15646,-1,-1,@columnname)  
   return(1)  
  end  
  
        declare ms_crs_syscom  CURSOR LOCAL  
        FOR select text from syscomments where id = @objid and encrypted = 0 and number =  
                        (select column_id from sys.columns where name = @columnname and object_id = @objid)  
                        order by number,colid  
        FOR READ ONLY  
  
    end  
else if @objid < 0 -- Handle system-objects  
 begin  
  -- Check count of rows with text data  
  if (select count(*) from master.sys.syscomments where id = @objid and text is not null) = 0  
   begin  
    raiserror(15197,-1,-1,@objname)  
    return (1)  
   end  
     
  declare ms_crs_syscom CURSOR LOCAL FOR select text from master.sys.syscomments where id = @objid  
   ORDER BY number, colid FOR READ ONLY  
 end  
else  
    begin  
        /*  
        **  Find out how many lines of text are coming back,  
        **  and return if there are none.  
        */  
        if (select count(*) from syscomments c, sysobjects o where o.xtype not in ('S', 'U')  
            and o.id = c.id and o.id = @objid) = 0  
                begin  
                        raiserror(15197,-1,-1,@objname)  
                        return (1)  
                end  
  
        if (select count(*) from syscomments where id = @objid and encrypted = 0) = 0  
                begin  
                        raiserror(15471,-1,-1,@objname)  
                        return (0)  
                end  
  
  declare ms_crs_syscom  CURSOR LOCAL  
  FOR select text from syscomments where id = @objid and encrypted = 0  
    ORDER BY number, colid  
  FOR READ ONLY  
  
    end  
  
/*  
**  else get the text.  
*/  
select @LFCR = 2  
select @LineId = 1  
  
  
OPEN ms_crs_syscom  
  
FETCH NEXT from ms_crs_syscom into @SyscomText  
  
WHILE @@fetch_status >= 0  
begin  
  
    select  @BasePos    = 1  
  select  @CurrentPos = 1  
    select  @TextLength = LEN(@SyscomText)  
  
    WHILE @CurrentPos  != 0  
    begin  
        --Looking for end of line followed by carriage return  
        select @CurrentPos =   CHARINDEX(char(13)+char(10), @SyscomText, @BasePos)  
  
        --If carriage return found  
        IF @CurrentPos != 0  
        begin  
            /*If new value for @Lines length will be > then the  
            **set length then insert current contents of @line  
            **and proceed.  
            */  
            while (isnull(LEN(@Line),0) + @BlankSpaceAdded + @CurrentPos-@BasePos + @LFCR) > @DefinedLength  
            begin  
                select @AddOnLen = @DefinedLength-(isnull(LEN(@Line),0) + @BlankSpaceAdded)  
                INSERT #CommentText VALUES  
                ( @LineId,  
                  isnull(@Line, N'') + isnull(SUBSTRING(@SyscomText, @BasePos, @AddOnLen), N''))  
                select @Line = NULL, @LineId = @LineId + 1,  
                       @BasePos = @BasePos + @AddOnLen, @BlankSpaceAdded = 0  
            end  
            select @Line    = isnull(@Line, N'') + isnull(SUBSTRING(@SyscomText, @BasePos, @CurrentPos-@BasePos + @LFCR), N'')  
            select @BasePos = @CurrentPos+2  
            INSERT #CommentText VALUES( @LineId, @Line )  
            select @LineId = @LineId + 1  
            select @Line = NULL  
        end  
        else  
        --else carriage return not found  
        begin  
            IF @BasePos <= @TextLength  
            begin  
                /*If new value for @Lines length will be > then the  
                **defined length  
                */  
                while (isnull(LEN(@Line),0) + @BlankSpaceAdded + @TextLength-@BasePos+1 ) > @DefinedLength  
                begin  
                    select @AddOnLen = @DefinedLength - (isnull(LEN(@Line),0) + @BlankSpaceAdded)  
                    INSERT #CommentText VALUES  
                    ( @LineId,  
                      isnull(@Line, N'') + isnull(SUBSTRING(@SyscomText, @BasePos, @AddOnLen), N''))  
                    select @Line = NULL, @LineId = @LineId + 1,  
                        @BasePos = @BasePos + @AddOnLen, @BlankSpaceAdded = 0  
                end  
                select @Line = isnull(@Line, N'') + isnull(SUBSTRING(@SyscomText, @BasePos, @TextLength-@BasePos+1 ), N'')  
                if LEN(@Line) < @DefinedLength and charindex(' ', @SyscomText, @TextLength+1 ) > 0  
                begin  
                    select @Line = @Line + ' ', @BlankSpaceAdded = 1  
                end  
            end  
        end  
    end  
  
 FETCH NEXT from ms_crs_syscom into @SyscomText  
end  
  
IF @Line is NOT NULL  
    INSERT #CommentText VALUES( @LineId, @Line )  

declare @lineNo int = 0;
declare @lineCnt int = 0;
declare @printtext varchar(max);
select @lineCnt = COUNT( LineId ) from #CommentText;
while( @lineNo < @lineCnt )
begin
	set @lineNo = @lineNo + 1;
	 
	select @printtext = Text from #CommentText
	where LineId = @lineNo ;
	
	print( @printtext );
end 
CLOSE  ms_crs_syscom  
DEALLOCATE  ms_crs_syscom  
  
DROP TABLE  #CommentText  
  
return (0) -- sp_helptext  

/*  프로시져 권한 등록
--grant execute on sp_helptxt to econnet
*/

/*  등록 후 별도로 시스템 프로시져로 등록함!
--USE [master]
--GO
--exec sp_ms_marksystemobject 'sp_helptxt' 
*/

";

            return qry;
        }
        
        string GET_SP_JSFW_TABLE_COLUMN_INFO()
        {
            string qry = @"
CREATE PROC [dbo].[SP_JSFW_TABLE_COLUMN_INFO]        
(        
	@TABLE_NAME nvarchar(776) = null
)        
AS    
     
 declare @OBJTYPE nvarchar(776) = ''     
      
 select  
 @OBJTYPE              = REPLACE( REPLACE( replace( replace(substring(v.name,5,31) , 'user ', '' ), 'table ', ''), 'STORED ', '' ),'inline ', '' )
-- replace(substring(v.name,5,31) , 'user ', '' ), * 
 from sys.all_objects o, master.dbo.spt_values v  
 where o.object_id = object_id(@TABLE_NAME) and o.type = substring(v.name,1,2) collate database_default and v.type = 'O9T'  

print @objtype

SELECT  '                   OBJECT INFO' [desc], A.OBJECT_ID, A.OBJECT_NAME, UPPER( ISNULL( B.objtype, @objtype )) as objtype, isnull(B.value, '') as value
FROM (
	SELECT  OBJECT_ID(@TABLE_NAME) AS [OBJECT_ID], @TABLE_NAME [OBJECT_NAME]
) A
LEFT OUTER JOIN
(
	SELECT OBJECT_ID(OBJNAME) OBJECT_ID , objtype, objname , name, value  
	FROM fn_listextendedproperty (NULL, 'schema', Schema_Name(), upper(@OBJTYPE), @TABLE_NAME, default, default)
	where name = 'MS_Description'
) B
ON A.[OBJECT_ID] = B.[OBJECT_ID]
 
 
IF UPPER(@OBJTYPE) = 'TABLE' BEGIN
     
 select distinct  'TABLE COLUMNS LIST' [desc], 
  c.object_id ,-- c.name, 
  c.COLUMN_ID,     
  CONVERT( VARCHAR,  c.NAME ) AS[NAME],  
  isnull(e.value,'') ExName,                
  ltrim( rtrim( case when isnull( index_col( @TABLE_NAME , i.index_id, i.index_column_id ),'') <> '' then 'PK' else '' end + ' '+    
  case when isnull( f.constraint_column_id , '' ) <> '' then 'FK' else '' end )) as [KEY] ,   
  case when ( c.is_nullable = 0 ) then 'N' else '' end [IsNull],   
  --isnull( t.name ,'') TypeName, 
  UPPER(t.NAME) AS [DTYPE] ,                    
  CASE WHEN UPPER(t.NAME) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR','TEXT' )  THEN CASE c.MAX_LENGTH WHEN -1 THEN '(MAX)'         
          ELSE '('+ CONVERT( VARCHAR, c.MAX_LENGTH / ( CASE WHEN LEFT(t.NAME,1) = 'N' THEN 2 ELSE 1 END ) ) +')'          
        END         
   WHEN UPPER(t.NAME) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.SCALE)+')'        
   ELSE ''         
  END AS [LEN],                    
  '-- ' + isnull( CONVERT( VARCHAR,  e.VALUE  ) , c.Name ) AS [DESC],                    
                     
  '@'+CONVERT( VARCHAR,  c.NAME )+' ' + UPPER(t.NAME) +  CASE WHEN UPPER(t.NAME) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR') THEN CASE c.MAX_LENGTH WHEN -1 THEN '(MAX)'         
            ELSE '('+ CONVERT( VARCHAR, c.MAX_LENGTH / ( CASE WHEN LEFT(t.NAME,1) = 'N' THEN 2 ELSE 1 END ) ) +')'         
          END         
           WHEN UPPER(t.NAME) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.SCALE)+')'        
           ELSE ''         
      END +         
   ', -- ' + isnull( CONVERT( VARCHAR,  e.VALUE  ) , '') AS  PROC_PRMS,        
                  
  '@'+CONVERT( VARCHAR,  c.NAME + ',' ) AS [PRMS] ,               
               
  'AND'  AS [AND],              
  CONVERT( VARCHAR,  c.NAME + ' = ' ) + '@'+CONVERT( VARCHAR,  c.NAME  ) AS [WPRMS]            
             
 from sys.columns c left outer join sys.extended_properties e        
 on c.Object_id = e.MAJOR_ID and c.Column_Id = e.Minor_id and e.CLASS = 1        
 left outer join SYS.TYPES t          
 on c.SYSTEM_TYPE_ID = t.SYSTEM_TYPE_ID AND t.NAME <> 'SYSNAME'        
 left outer join sys.index_columns i    
 on c.object_id = i.object_id and c.column_id = i.column_id    
 left outer join  sys.foreign_key_columns f    
 on c.object_id = f.parent_object_id and c.column_id = f.parent_column_id    
 where c.Object_id = OBJECT_ID( @TABLE_NAME )        
 ORDER BY C.column_id    
END
ELSE	BEGIN
  /* - Start : GoodSen .cs sbFormInit() Source*/
     declare @execdeclare varchar(max) = 'DECLARE 
            '
     declare @execcmd varchar(max) = 'exec '+ @TABLE_NAME + ' '
     declare @csp varchar(max) = ''
     declare @cs varchar(max) = ''
	 declare @prmsName varchar(50) = 'prms';
     declare @spName varchar(200) = '';

     set @spName = case when CHARINDEX( 'SELECT', upper( @TABLE_NAME ), 1 ) > 1 then 'MBaseClass.SelectSpName = ""' + @TABLE_NAME + '""'
                        when CHARINDEX( 'INSERT', upper( @TABLE_NAME ), 1 ) > 1 then 'MBaseClass.InsertSpName = ""' + @TABLE_NAME + '""'
                        when CHARINDEX( 'UPDATE', upper( @TABLE_NAME ), 1 ) > 1 then 'MBaseClass.UpdateSpName = ""' + @TABLE_NAME + '""'
                        when CHARINDEX( 'DELETE', upper( @TABLE_NAME ), 1 ) > 1 then 'MBaseClass.DeleteSpName = ""' + @TABLE_NAME + '""'
                        else @spName
                   end

     set @prmsName = case when CHARINDEX( 'SELECT', upper( @TABLE_NAME ), 1 ) > 1 then 'SelectParams' 
                          when CHARINDEX( 'INSERT', upper( @TABLE_NAME ), 1 ) > 1 then 'InsertParams' 
                          when CHARINDEX( 'UPDATE', upper( @TABLE_NAME ), 1 ) > 1 then 'UpdateParams' 
                          when CHARINDEX( 'DELETE', upper( @TABLE_NAME ), 1 ) > 1 then 'DeleteParams' 
                          else @prmsName
                     end


     ;with cs_prms_tb as (
     select distinct   
      c.parameter_id,                     
      CONVERT( VARCHAR,  c.NAME ) AS[NAME], 
      convert( varchar(1000),  isnull(e.value, (
                          select top 1 isnull( ex.value, '' ) 
                    from sys.columns col
                    inner join ( 
                        select major_id, minor_id, value 
                        from  sys.extended_properties 
                        where name = 'MS_Description' and value is not null
                    ) as ex
                    on ex.major_id = col.object_id and  ex.minor_id = col.column_id
                    where upper( col.name) = upper( rtrim( REPLACE( c.NAME, '@', '') ))    
      ))) as ExName,
      case when upper(t.NAME) = 'CHAR'      then 'Char'
           when upper(t.NAME) = 'VARCHAR'   then 'VarChar'
           when upper(t.NAME) = 'NUMERIC'   then 'Decimal'
           when upper(t.NAME) = 'INT'       then 'Int'
           when upper(t.NAME) = 'BIGINT'    then 'BigInt'
           else ''
      end AS [sqltype], 
    upper(t.NAME) as [typename],
    CASE WHEN UPPER(t.NAME) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR','TEXT' )  THEN CASE c.MAX_LENGTH WHEN -1 THEN 'MAX' ELSE CONVERT( VARCHAR, c.MAX_LENGTH / ( CASE WHEN LEFT(t.NAME,1) = 'N' THEN 2 ELSE 1 END ) ) END         
         WHEN UPPER(t.NAME) IN ('NUMERIC','DECIMAL' ) THEN CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.SCALE)       
         ELSE ''         
    END AS [LEN],
    c.is_output as ISOUTPUT                
             
     from sys.parameters c left outer join sys.extended_properties e        
     on c.Object_id = e.MAJOR_ID and c.parameter_id = e.Minor_id and e.CLASS = 2        
     left outer join SYS.TYPES t          
     on c.SYSTEM_TYPE_ID = t.SYSTEM_TYPE_ID AND t.NAME <> 'SYSNAME'        
     left outer join sys.index_columns i    
     on c.object_id = i.object_id and c.parameter_id = i.column_id    
     left outer join  sys.foreign_key_columns f    
     on c.object_id = f.parent_object_id and c.parameter_id = f.parent_column_id    
      
     where c.Object_id = OBJECT_ID( @TABLE_NAME )   
    -- ORDER BY C.parameter_id   
     ) 
      
     select @cs +=  @prmsName + '.Add(""'+ NAME +'"", ParameterDirection.' + ( case when ISOUTPUT = 0 then  'Input' else 'Output' end ) +', 0, 0, '+ ( case when ISOUTPUT = 0 then  '0' else [LEN] end ) +', SqlDbType.'+ sqltype +'); /*'+isnull(ExName,'')+'*/' + char(10),
            @csp += 'e.RowData.Parameters['+ convert( varchar,  ROW_NUMBER() over( order by parameter_id )-1)+'].Value/*'+isnull(ExName,'')+'*/ = grdData[ e.RowData.RowIndex, uc'+ replace( NAME, '@', '') +'];' + char(10),
            @execdeclare += NAME + ' ' + [typename] + case when rtrim( [LEN] ) = '' then '' else QUOTENAME( [LEN] , '(') end + ' = null' +  ',
            ',
           @execcmd += NAME + ( case when ISOUTPUT = 0 then  '' else ' = ' + NAME +' Output' end ) + ', '
     from cs_prms_tb
     order by parameter_id
      
     set @execdeclare = left ( @execdeclare, len( @execdeclare ) - len (',
            ')) 
     set @execcmd = left ( @execcmd, len( @execcmd ) - len (', ')) 
     
     print @execdeclare + char(10)
     print @execcmd + char(10) + char(10)

     print @spName
     print @cs  
     print @csp  
	 /* - End : GoodSen .cs sbFormInit() Source*/

 select distinct   '           PARAMETER LIST' [desc],  
  c.object_id ,-- c.name,   
  c.parameter_id,                     
  CONVERT( VARCHAR,  c.NAME ) AS[NAME], 
  isnull(e.value,'') ExName ,
  ltrim( rtrim( case when isnull( index_col( @TABLE_NAME , i.index_id, i.index_column_id ),'') <> '' then 'PK' else '' end + ' '+    
  case when isnull( f.constraint_column_id , '' ) <> '' then 'FK' else '' end )) as [KEY] ,   
    case when ( c.default_value = 0 ) then 'N' else '' end [IsNull],   
  --isnull( t.name ,'') TypeName,  
  UPPER(t.NAME) AS [DTYPE] ,                    
  CASE WHEN UPPER(t.NAME) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR','TEXT' )  THEN CASE c.MAX_LENGTH WHEN -1 THEN '(MAX)'         
          ELSE '('+ CONVERT( VARCHAR, c.MAX_LENGTH / ( CASE WHEN LEFT(t.NAME,1) = 'N' THEN 2 ELSE 1 END ) ) +')'          
        END         
   WHEN UPPER(t.NAME) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.SCALE)+')'        
   ELSE ''         
  END AS [LEN],                    
  '-- ' + isnull( CONVERT( VARCHAR,  e.VALUE  ) , c.Name ) AS [DESC],                    
                     
  CONVERT( VARCHAR,  c.NAME )+' ' + UPPER(t.NAME) +  CASE WHEN UPPER(t.NAME) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR') THEN CASE c.MAX_LENGTH WHEN -1 THEN '(MAX)'         
            ELSE '('+ CONVERT( VARCHAR, c.MAX_LENGTH / ( CASE WHEN LEFT(t.NAME,1) = 'N' THEN 2 ELSE 1 END ) ) +')'         
          END         
           WHEN UPPER(t.NAME) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.SCALE)+')'        
           ELSE ''         
      END +         
   ', -- ' + isnull( CONVERT( VARCHAR,  e.VALUE  ) , '') AS  PROC_PRMS,        
                  
  CONVERT( VARCHAR,  c.NAME + ',' ) AS [PRMS] ,               
               
  'AND'  AS [AND],              
  CONVERT( VARCHAR,  c.NAME + ' = ' ) + '@'+CONVERT( VARCHAR,  c.NAME  ) AS [WPRMS]            
              
 from sys.parameters c left outer join sys.extended_properties e        
 on c.Object_id = e.MAJOR_ID and c.parameter_id = e.Minor_id and e.CLASS = 2        
 left outer join SYS.TYPES t          
 on c.SYSTEM_TYPE_ID = t.SYSTEM_TYPE_ID AND t.NAME <> 'SYSNAME'        
 left outer join sys.index_columns i    
 on c.object_id = i.object_id and c.parameter_id = i.column_id    
 left outer join  sys.foreign_key_columns f    
 on c.object_id = f.parent_object_id and c.parameter_id = f.parent_column_id    
 where c.Object_id = OBJECT_ID( @TABLE_NAME )        
 ORDER BY C.parameter_id   
 
             if( @objtype = 'procedure' ) begin

                        --declare @grdValidate varchar(max) = '' 
                        --set  @grdValidate += 'var IsOverDataLength = new Func<string, int, bool>((txt, length) => {' + char(10)
                        --set  @grdValidate += '		int _dataLength = Encoding.Default.GetByteCount(txt); //바이트 카운트 : 영문(1), 한글(2) ' + char(10)
                        --set  @grdValidate += '		return length < _dataLength;' + char(10)
                        --set  @grdValidate += '});' + char(10) + char(10)
						  
                        --set  @grdValidate += 'switch (jCol)' + char(10)
                        --set  @grdValidate += '{' + char(10)

                        --SELECT  @grdValidate += ' case uc' + name + ':' + char(10) + 
                        --'     if (grdData[jRow, jCol] != null && IsOverDataLength(("""" + grdData[jRow, jCol]).Trim(), '+ convert( varchar, max_length ) +'))' + char(10) + 
                        --'     {' + char(10) + 
                        --'		    Messages.Display(grdData.Cols[jCol].Caption + ""(은)는 '+ convert( varchar, max_length ) +'자리이내 문자열을 입력하셔야 됩니다."");' + char(10) +  
                        --'		    return false;' + char(10) +
                        --'	    }' + char(10) +
                        --'	break;' + char(10) + char(10)
                        --FROM SYS.DM_EXEC_DESCRIBE_FIRST_RESULT_SET_FOR_OBJECT(object_id(@TABLE_NAME), 0)
                        --where system_type_name like '%char%'
                        --set @grdValidate += '}' + char(10)
                        --print @grdValidate
 
                        ----- C1FlexGrid MaxLength 체크.
						 declare @grdValidate varchar(max) = '' + char(10)
						 set  @grdValidate += 'grdData.SetupEditor += (gs, ge) => {' + char(10)
						 set  @grdValidate += '		System.Windows.Forms.TextBox tb = grdData.Editor as System.Windows.Forms.TextBox;' + char(10)
						 set  @grdValidate += '		if (tb == null) return;' + char(10) 
						 set  @grdValidate += '		switch (ge.Col)' + char(10)
						 set  @grdValidate += '		{' + char(10) 
						 SELECT  @grdValidate += '			case uc' + name + ':' + char(10) + 
								 '			    tb.MaxLength = '+ convert( varchar, case when max_length = -1 then TYPEPROPERTY( type_name( system_type_id ), 'Precision' ) else  max_length end  ) +';' + char(10) +
                                 '			break;' + char(10)
                         FROM SYS.DM_EXEC_DESCRIBE_FIRST_RESULT_SET_FOR_OBJECT(object_id(@TABLE_NAME), 0) 
                         where system_type_name like '%char%' and name like '[^YN]%'
						 set @grdValidate += '		}' + char(10) 
                         set @grdValidate += '};' + char(10) 
                         print @grdValidate 

 
                         --프로시져 아웃정보
                       ; WITH TB_COLUMN AS
                            (
                                SELECT  A.COLUMN_NAME
                                    , MAX(B.VALUE) AS COLUM_COMMENT
                                FROM    INFORMATION_SCHEMA.COLUMNS A
                                LEFT JOIN SYS.COLUMNS COL ON object_id(A.TABLE_NAME) = COL.OBJECT_ID
                                                         AND A.ORDINAL_POSITION = COL.COLUMN_ID
                                LEFT JOIN SYS.EXTENDED_PROPERTIES  B ON B.major_id = object_id(A.TABLE_NAME)
                                                                    AND A.ORDINAL_POSITION = B.minor_id
                                WHERE   ISNULL(B.VALUE, '') <> '' AND A.COLUMN_NAME IN ( SELECT NAME FROM SYS.DM_EXEC_DESCRIBE_FIRST_RESULT_SET_FOR_OBJECT(object_id(@TABLE_NAME), 0) )
                                GROUP   BY A.COLUMN_NAME
                            )

                        SELECT  ROW_NUMBER() OVER(ORDER BY A.COLUMN_ORDINAL) AS IX_COLUMN
                            ,   A.NAME AS NM_COLUMN
							,   B.COLUM_COMMENT AS COLUMN_COMMENT
							,   A.SYSTEM_TYPE_NAME AS TY_COLUMN
							,   'const int uc' + A.NAME + ' = ' + CONVERT(NVARCHAR, ROW_NUMBER() OVER(ORDER BY A.COLUMN_ORDINAL)) + ';    // ' +
                                CASE WHEN A.NAME IN ('ID_INSERT', 'DT_INSERT', 'ID_UPDATE', 'DT_UPDATE')
									 THEN CASE A.NAME WHEN 'ID_INSERT' THEN '입력자ID' 
                                                      WHEN 'DT_INSERT' THEN '입력일자' 
                                                      WHEN 'ID_UPDATE' THEN '수정자ID' 
                                                      WHEN 'DT_UPDATE' THEN '수정일자' 
                                           END
                                     ELSE CASE WHEN ISNULL(B.COLUM_COMMENT, '') = '' THEN A.NAME ELSE CONVERT(NVARCHAR, B.COLUM_COMMENT) END
                                END AS COLUMN_CONST
                            ,  'uc' + A.NAME as grdData_Columns
                        FROM SYS.DM_EXEC_DESCRIBE_FIRST_RESULT_SET_FOR_OBJECT(object_id(@TABLE_NAME), 0) AS A 
                        LEFT JOIN TB_COLUMN B ON A.NAME = B.COLUMN_NAME 
                        ORDER BY COLUMN_ORDINAL; 
            end


  if  UPPER(@OBJTYPE) = 'FUNCTION' or UPPER(@OBJTYPE) = 'SCALAR FUNCTION' begin
  
	  select distinct   'RESULT COLUMNS LIST' [desc],    
	  c.object_id ,-- c.name,    
	  c.COLUMN_ID,                     
	  CONVERT( VARCHAR,  c.NAME ) AS[NAME], 
	  isnull(e.value,'') ExName ,      
	  ltrim( rtrim( case when isnull( index_col( @TABLE_NAME , i.index_id, i.index_column_id ),'') <> '' then 'PK' else '' end + ' '+    
	  case when isnull( f.constraint_column_id , '' ) <> '' then 'FK' else '' end )) as [KEY] ,   
	  case when ( c.is_nullable = 0 ) then 'N' else '' end [IsNull],   
	  --isnull( t.name ,'') TypeName,     
	  UPPER(t.NAME) AS [DTYPE] ,                    
	  CASE WHEN UPPER(t.NAME) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR','TEXT' )  THEN CASE c.MAX_LENGTH WHEN -1 THEN '(MAX)'         
			  ELSE '('+ CONVERT( VARCHAR, c.MAX_LENGTH / ( CASE WHEN LEFT(t.NAME,1) = 'N' THEN 2 ELSE 1 END ) ) +')'          
			END         
	   WHEN UPPER(t.NAME) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.SCALE)+')'        
	   ELSE ''         
	  END AS [LEN],                    
	  '-- ' + isnull( CONVERT( VARCHAR,  e.VALUE  ) , c.Name ) AS [DESC],                    
	                     
	  '@'+CONVERT( VARCHAR,  c.NAME )+' ' + UPPER(t.NAME) +  CASE WHEN UPPER(t.NAME) IN ('CHAR','VARCHAR','NVARCHAR','NCHAR') THEN CASE c.MAX_LENGTH WHEN -1 THEN '(MAX)'         
				ELSE '('+ CONVERT( VARCHAR, c.MAX_LENGTH / ( CASE WHEN LEFT(t.NAME,1) = 'N' THEN 2 ELSE 1 END ) ) +')'         
			  END         
			   WHEN UPPER(t.NAME) IN ('NUMERIC','DECIMAL' ) THEN '('+ CONVERT(VARCHAR, c.PRECISION) + ',' + CONVERT(VARCHAR, c.SCALE)+')'        
			   ELSE ''         
		  END +         
	   ', -- ' + isnull( CONVERT( VARCHAR,  e.VALUE  ) , '') AS  PROC_PRMS,        
	                  
	  '@'+CONVERT( VARCHAR,  c.NAME + ',' ) AS [PRMS] ,               
	               
	  'AND'  AS [AND],              
	  CONVERT( VARCHAR,  c.NAME + ' = ' ) + '@'+CONVERT( VARCHAR,  c.NAME  ) AS [WPRMS]            
	          
	 from sys.columns c left outer join sys.extended_properties e        
	 on c.Object_id = e.MAJOR_ID and c.Column_Id = e.Minor_id and e.CLASS = 1        
	 left outer join SYS.TYPES t          
	 on c.SYSTEM_TYPE_ID = t.SYSTEM_TYPE_ID AND t.NAME <> 'SYSNAME'        
	 left outer join sys.index_columns i    
	 on c.object_id = i.object_id and c.column_id = i.column_id    
	 left outer join  sys.foreign_key_columns f    
	 on c.object_id = f.parent_object_id and c.column_id = f.parent_column_id    
	 where c.Object_id = OBJECT_ID( @TABLE_NAME )        
	 ORDER BY C.column_id    
 
 
 
 end
 
END  
";



            return qry;
        }
         
        public string GET_SP_JS_TABLE_SELECT()
        {
            string qry = @"
CREATE PROC [dbo].[sp_js_table_select]        
    @TABLE_NM NVARCHAR(776)  = ''       
as        
        
IF ( RTRIM( @TABLE_NM ) = '' OR @TABLE_NM IS NULL )
BEGIN
        
        PRINT ( ' ^ ______ ^' ) 
        RETURN ;
END

 

DECLARE @QUERY NVARCHAR( MAX )

SET @QUERY = '' 

;WITH TABLE_COLS ( COLUMN_ID, NAME ) AS
(
     SELECT  COLUMN_ID,
             NAME 
       FROM  SYS.COLUMNS
      WHERE  OBJECT_ID = OBJECT_ID( @TABLE_NM )
),COLS_ATTS ( NAME, HEADER ) AS
(
     SELECT  objname AS NAME,
             REPLACE( REPLACE( CAST( value AS VARCHAR( 100 ) ), '[' , '' ), ']' , '' ) AS HEADER
       FROM  fn_listextendedproperty( NULL , 'schema' , Schema_Name() , 'table' , @TABLE_NM , 'column' , default )
 )

SELECT  @QUERY = ISNULL( @QUERY , '' ) + ISNULL( QUOTENAME( C.NAME ), '' ) + ISNULL( ' AS [<' + ISNULL( NULLIF( RTRIM( CONVERT( VARCHAR( 100 ), A.HEADER ) ), '' ), C.NAME ) + '(' + C.NAME + ')>]' , ' AS ' + C.NAME ) +', '
  FROM  TABLE_COLS C 
  LEFT  
 OUTER  JOIN COLS_ATTS A
    ON  C.NAME = A.NAME COLLATE Korean_Wansung_CI_AS

EXEC ( 'SELECT TOP 100 ' + @QUERY + ' '''' AS [[END__] ' + ' FROM ' + @TABLE_NM )
 
";
            return qry;
        } 
    }
}
